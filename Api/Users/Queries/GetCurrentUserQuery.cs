
using MediatR;
using Signup.API.Dtos;
using Signup.API.Models;
using Signup.API.Users.Repos;

namespace Signup.API.Users.Queries
{
    public class GetCurrentUserQuery : IRequest<UserDto>
    {

    }

    public class GetCurrentUserQueryHandler : RequestHandler<GetCurrentUserQuery, UserDto>
    {
        private readonly IUserRepository _repo;

        public GetCurrentUserQueryHandler(IUserRepository repo)
        {
            _repo = repo;
        }

        protected override UserDto Handle(GetCurrentUserQuery query)
        {
            var t = _repo.GetCurrentUser().Map();
            return t;
        }
    }
}