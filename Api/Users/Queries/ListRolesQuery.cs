using System.Collections.Generic;
using MediatR;
using Signup.API.Dtos;
using Signup.API.Users.Repos;

namespace Signup.API.Users.Queries
{

    public class ListRolesQuery : IRequest<IEnumerable<UserRoleDto>>
    {

    }

    public class ListRolesQueryHandler : RequestHandler<ListRolesQuery, IEnumerable<UserRoleDto>>
    {
        private readonly IUserRepository _repo;

        public ListRolesQueryHandler(IUserRepository repo)
        {
            _repo = repo;
        }

        protected override IEnumerable<UserRoleDto> Handle(ListRolesQuery query)
        {
            return _repo.ListRoles();
        }
    }
}