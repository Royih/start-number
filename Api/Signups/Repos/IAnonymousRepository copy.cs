

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
        Task<CommandResultDto> SignUp(SignUpDto signUpData);

    }
}