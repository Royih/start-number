

using System.Collections.Generic;
using System.Threading.Tasks;
using Signup.API.Signups.Dtos;

namespace Signup.API.Users.Repos
{
    public interface ISignupRepository
    {
        Task<IEnumerable<SignUpsForEventDto>> ListSignups(string eventId);

    }
}