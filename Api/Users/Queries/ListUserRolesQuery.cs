using System;
using System.Collections.Generic;
using System.Security;
using MediatR;
using Signup.API.Dtos;
using Signup.API.Models;
using Signup.API.Users.Repos;

namespace Signup.API.Users.Queries
{

    public class ListUserRolesQuery : IRequest<IEnumerable<UserRoleDto>>
    {
        public Guid Id { get; set; }
    }

    public class ListUserRolesQueryHandler : RequestHandler<ListUserRolesQuery, IEnumerable<UserRoleDto>>
    {
        private readonly IUserRepository _repo;
        private readonly ICurrentUser _currentUser;

        public ListUserRolesQueryHandler(IUserRepository repo, ICurrentUser currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }

        protected override IEnumerable<UserRoleDto> Handle(ListUserRolesQuery query)
        {
            if (!_currentUser.HasRole(Roles.Admin) && _currentUser.Id != query.Id)
            {
                throw new SecurityException($"User {_currentUser.UserName} does not have access to user with id {query.Id}");
            }
            return _repo.ListUsersRoles(query.Id);
        }
    }
}