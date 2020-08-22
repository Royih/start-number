using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Signup.API.Models
{
    public class CurrentUser : ICurrentUser
    {
        public string UserName
        {
            get;
            private set;
        }

        public string TenantId { get; }
        public Guid? Id { get; private set; }
        public Roles[] Roles { get; private set; }

        public CurrentUser(IHttpContextAccessor ctx)
        {
            if (ctx.HttpContext != null)
            {
                if (ctx.HttpContext.User.Identity.IsAuthenticated)
                {
                    UserName = ctx.HttpContext.User.Identity.Name;
                    Id = ctx.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value.AsGuid();
                    Roles = GetRoles(ctx.HttpContext.User.Claims.Where(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(x => x.Value)).ToArray();
                }

            }

        }
        private static IEnumerable<Roles> GetRoles(IEnumerable<string> rolesAsString)
        {
            foreach (var roleAsString in rolesAsString)
            {
                if (Enum.TryParse(typeof(Roles), roleAsString, out var role))
                {
                    yield return (Roles)role;
                }
            }
        }

        public bool HasRole(Roles role)
        {
            return Roles.Contains(role);
        }

        public bool HasAnyRole(Roles[] roles)
        {
            return Roles.Any(x => roles.Any(y => y == x));
        }
    }
}