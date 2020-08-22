using System;
using Signup.API.Models;

namespace Signup.API.Dtos
{
    public class SignUpDto
    {
        public string TenantKey { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public bool AllowUsToContactPersonByEmail { get; set; } = true;
        public bool PreviouslyParticipated { get; set; }
    }

}