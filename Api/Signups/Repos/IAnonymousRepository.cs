

using System.Collections.Generic;
using System.Threading.Tasks;
using Signup.API.Dtos;
using Signup.API.Models;
using Signup.API.Signups.Dtos;

namespace Signup.API.Users.Repos
{
    public interface IAnonymousRepository

    {
        Task<IEnumerable<ActiveEventDto>> ListActiveEvents();
        Task<CommandResultDto<string>> SignUp(SignUpDto signUpData);
        Task<EventDataDto> GetEventData(string key);
        Task<byte[]> GetStartNumberPdf(string eventId, string personId);
        byte[] GetStartNumberPdf(SignupRecord signup);
        Task<SignupRecord> GetSignup(string signupId);
    }
}