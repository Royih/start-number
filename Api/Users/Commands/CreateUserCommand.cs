using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Signup.API.Dtos;
using Signup.API.Models;
using Signup.API.Users.Repos;

namespace Signup.API.Users.Commands
{

    public class CreateUserCommand : IRequest<CommandResultDto>
    {

        public UserDto User { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordRepeat { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CommandResultDto>
    {

        private readonly IUserRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUser _currentUser;

        public CreateUserCommandHandler(IUserRepository repo, UserManager<ApplicationUser> userManager, ICurrentUser currentUser)
        {
            _repo = repo;
            _userManager = userManager;
            _currentUser = currentUser;
        }

        public async Task<CommandResultDto> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            return await _repo.CreateUser(command.User, command.NewPassword);

        }
    }

}