
namespace Signup.API.Signups.Dtos
{
    public class SignUpsForEventDto
    {
        public int StartNumber { get; set; }
        public string PersonId { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public bool AllowUsToContactPersonByEmail { get; set; }

    }

}