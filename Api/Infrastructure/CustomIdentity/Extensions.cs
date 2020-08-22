using System;
using System.Text;
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Signup.API.Models;

namespace Signup.API.Infrastructure.CustomIdentity
{

    public static class Extensions
    {
        private const string AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789æøåÆØÅ-@. _";
        private static SymmetricSecurityKey _signingKey;

        public static void AddCustomIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["SecretKey"]));

            services.AddIdentity<ApplicationUser, MongoIdentityRole>(options =>
                {
                    options.User.AllowedUserNameCharacters = AllowedUserNameCharacters;
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                })
                .AddMongoDbStores<ApplicationUser, MongoIdentityRole, Guid>(configuration.GetMongoConnectionString(), configuration.GetMongoDatabaseName())
                .AddDefaultTokenProviders();

            services.AddControllersWithViews(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    //.RequireClaim("role", "User")
                    .Build();

                config.Filters.Add(new AuthorizeFilter(policy));
                //config.Filters.Add(typeof(CustomExceptionFilter));
            });

            // .AddJsonOptions(x =>
            // {
            //     x.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //     x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            //     x.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            // });

            var jwtAppSettingOptions = configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

            });

            services.AddAuthentication(x =>
                {
                    // One of these two was important to return 403 instead of redirect to access denied page (Razor-stuff?)
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {

                    options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                    options.RequireHttpsMetadata = true; //_environment.IsProduction();
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                        ValidateAudience = true,
                        ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                        ValidateIssuerSigningKey = true,

                        ValidateLifetime = true,

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["SecretKey"])),
                        ClockSkew = TimeSpan.Zero
                    };

                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("UserPolicy",
                    policy => policy.RequireRole("User"));
            });
        }
    }
}