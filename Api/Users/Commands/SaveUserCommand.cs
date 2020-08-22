using System.Collections.Generic;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Signup.API.Dtos;
using Signup.API.Models;
using Signup.API.Users.Repos;

namespace Signup.API.Users.Commands
{

    public class SaveUserCommand : IRequest<CommandResultDto>
    {
        public UserDto User { get; set; }
        public IEnumerable<UserRoleDto> Roles { get; set; }

    }

    public class SaveUserCommandHandler : IRequestHandler<SaveUserCommand, CommandResultDto>
    {
        private readonly IUserRepository _repo;
        private readonly ICurrentUser _currentUser;

        public SaveUserCommandHandler(IUserRepository repo, ICurrentUser currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }

        public async Task<CommandResultDto> Handle(SaveUserCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.HasRole(Roles.Admin) && _currentUser.Id != command.User.Id)
            {
                throw new SecurityException($"User {_currentUser.UserName} does not have access to save user with id {command.User.Id}");
            }
            return await _repo.SaveUser(command.User, command.Roles);
        }
    }
}