using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Signup.API.Models;
using Signup.API.Users.Repos;

namespace Signup.API.Infrastructure.CustomIdentity
{
    [Route("api/[controller]")]
    [EnableCors()]
    [ApiController]
    public class JwtController : Controller
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly JsonSerializerSettings _serializerSettings;

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _repository;

        public JwtController(IOptions<JwtIssuerOptions> jwtOptions, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IUserRepository repository)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
            _repository = repository;
            ThrowIfInvalidOptions(_jwtOptions);

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        public class Creds
        {
            public string UserName { get; set; }
            public string Password { get; set; }

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostLogin(Creds creds)
        {
            //todo: Ensure that these input values are not logged. 
            var user = await SignInUser(creds.UserName, creds.Password);
            if (user == null)
            {

                return BadRequest("Invalid credentials");
            }
            var identity = await _repository.GetClaimsIdentity(user.UserName);
            var jwtAccessToken = await GetAccessToken(identity, user);
            var jwtRefreshToken = await GetRefreshToken(identity, user);
            // Create the JWT security tokens and encode it.
            var encodedJwtAccess = new JwtSecurityTokenHandler().WriteToken(jwtAccessToken);
            var encodedJwtRefresh = new JwtSecurityTokenHandler().WriteToken(jwtRefreshToken);

            // Serialize and return the response
            var response = new
            {
                access_token = encodedJwtAccess,
                refresh_token = encodedJwtRefresh,
                expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
            };
            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            _repository.AddRefreshToken(encodedJwtRefresh, user, jwtRefreshToken.ValidTo, this.HttpContext.Connection.RemoteIpAddress.ToString());
            return new OkObjectResult(json);
        }

        public class RefreshMyToken
        {
            public string RefreshToken { get; set; }
        }

        [HttpPost]
        [Route("RefreshAccessToken")]
        [AllowAnonymous]
        public async Task<IActionResult> PostRefreshAccessToken(RefreshMyToken refreshMyToken)
        {
            var token = new JwtSecurityToken(refreshMyToken.RefreshToken);
            var isRefreshToken = token.Claims.FirstOrDefault(x => x.Type == "RefreshToken")?.Value == "RefreshToken";
            if (!isRefreshToken)
            {
                throw new SecurityException("The given Refreshtoken is no RefreshToken");
            }
            var userIdAsString = token.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
            if (userIdAsString != null && Guid.TryParse(userIdAsString, out var userId))
            {
                var user = _repository.GetUser(userId);

                if (!_repository.ValidateRefreshToken(refreshMyToken.RefreshToken, user))
                {
                    return Unauthorized("Unknown or expired access token");
                }

                var identity = await _repository.GetClaimsIdentity(user.UserName);
                var newJwtAccessToken = await GetAccessToken(identity, user);

                var encodedJwtAccess = new JwtSecurityTokenHandler().WriteToken(newJwtAccessToken);
                // Serialize and return the response
                var response = new
                {
                    access_token = encodedJwtAccess,
                    expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
                };

                var json = JsonConvert.SerializeObject(response, _serializerSettings);
                return new OkObjectResult(json);
            }
            else
            {
                throw new Exception("UserId missing from Token");
            }

        }

        public class RemoveRefreshToken
        {
            public string RefreshToken { get; set; }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("RemoveRefreshToken")]
        public IActionResult PostRemoveRefreshToken(RemoveRefreshToken removeRefreshToken)
        {
            var removed = _repository.RemoveRefreshToken(removeRefreshToken.RefreshToken);
            return new OkObjectResult(removed);
        }

        private async Task<JwtSecurityToken> GetAccessToken(ClaimsIdentity identity, ApplicationUser user)
        {
            var claims = (await GetClaims(identity.Name, user)).ToList();

            // The user object that will be embedded in the access token.
            var userObject = JsonConvert.SerializeObject(GetLimitedUserObject(identity, user), _serializerSettings);
            claims.Add(new Claim("user", userObject));

            claims.AddRange(identity.FindAll("role"));
            var jwtAccessToken = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims.ToArray(),
                notBefore: _jwtOptions.NotBefore,
                expires: DateTime.UtcNow.Add(_jwtOptions.ValidFor),
                signingCredentials: _jwtOptions.SigningCredentials);
            return jwtAccessToken;
        }

        private async Task<JwtSecurityToken> GetRefreshToken(ClaimsIdentity identity, ApplicationUser user)
        {
            var claims = (await GetClaims(identity.Name, user)).ToList();
            claims.Add(new Claim("RefreshToken", "RefreshToken"));
            var jwtRefreshToken = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims.ToArray(),
                notBefore: _jwtOptions.NotBefore,
                expires: DateTime.UtcNow.AddMonths(3),
                signingCredentials: _jwtOptions.SigningCredentials);
            return jwtRefreshToken;
        }

        private async Task<Claim[]> GetClaims(string userName, ApplicationUser user)
        {
            return new[]
            {
                new Claim("UserId", user.Id.ToString()),
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                    new Claim(JwtRegisteredClaimNames.Iat,
                        ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(),
                        ClaimValueTypes.Integer64),
            };

        }

        private dynamic GetLimitedUserObject(ClaimsIdentity user, ApplicationUser userProfile)
        {
            return new
            {
                id = userProfile.Id.ToString(),
                name = userProfile.FullName,
                roles = userProfile.Roles,
                claims = user.Claims.Select(x => new { type = x.Type.ToString(), value = x.Value })
            };
        }

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date) => (long)Math.Round((date.ToUniversalTime() -
                new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
            .TotalSeconds);

        private async Task<ApplicationUser> SignInUser(string userName, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(userName, password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var usr = await _userManager.FindByNameAsync(userName);
                return usr;
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ApplicationUser>(null);
        }

    }
}