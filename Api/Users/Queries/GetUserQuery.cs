using System;
using System.Security;
using MediatR;
using Signup.API.Dtos;
using Signup.API.Models;
using Signup.API.Users.Repos;

namespace Signup.API.Users.Queries
{

    public class GetUserQuery : IRequest<UserDto>
    {
        public Guid Id { get; set; }
    }

    public class GetUserQueryHandler : RequestHandler<GetUserQuery, UserDto>
    {
        private readonly IUserRepository _repo;
        private readonly ICurrentUser _currentUser;

        public GetUserQueryHandler(IUserRepository repo, ICurrentUser currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }

        protected override UserDto Handle(GetUserQuery query)
        {
            if (!_currentUser.HasRole(Roles.Admin) && _currentUser.Id != query.Id)
            {
                throw new SecurityException($"User {_currentUser.UserName} does not have access to user with id {query.Id}");
            }
            var t = _repo.GetUser(query.Id).Map();
            return t;
        }
    }
}