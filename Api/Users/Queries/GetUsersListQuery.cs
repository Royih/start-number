using System.Collections.Generic;
using System.Linq;
using MediatR;
using Signup.API.Dtos;
using Signup.API.Models;
using Signup.API.Users.Repos;

namespace Signup.API.Users.Queries
{

    public class GetUsersListQuery : IRequest<IEnumerable<UserDto>>
    { }

    public class GetUsersListQueryHandler : RequestHandler<GetUsersListQuery, IEnumerable<UserDto>>
    {
        private readonly IUserRepository _repo;

        public GetUsersListQueryHandler(IUserRepository repo)
        {
            _repo = repo;
        }

        protected override IEnumerable<UserDto> Handle(GetUsersListQuery query)
        {
            var t = _repo.ListUsers().Select(x => x.Map());
            return t;
        }
    }
}