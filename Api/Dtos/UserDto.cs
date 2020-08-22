using System;
using Signup.API.Models;

namespace Signup.API.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public CultureDto Culture { get; set; }
    }

    public static class MappingExtensions
    {
        public static UserDto Map(this ApplicationUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Culture = Constants.GetCulture(user.Culture)
            };
        }
    }
}