using System.Security;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Signup.API.Dtos;
using Signup.API.Models;
using Signup.API.Users.Repos;

namespace Signup.API.Users.Commands
{

    public class UpdateOwnProfileCommand : IRequest<CommandResultDto>
    {
        public UserDto User { get; set; }
    }

    public class UpdateOwnProfileCommandHandler : IRequestHandler<UpdateOwnProfileCommand, CommandResultDto>
    {
        private readonly IUserRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;

        public UpdateOwnProfileCommandHandler(IUserRepository repo, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _userManager = userManager;
        }

        public async Task<CommandResultDto> Handle(UpdateOwnProfileCommand command, CancellationToken cancellationToken)
        {
            if (_repo.GetCurrentUser().Id.ToString() != command.User.Id.ToString())
            {
                throw new SecurityException("User is only allowed to update own profile!");
            }
            await _repo.SaveUser(command.User);

            return new CommandResultDto { Success = true };
        }
    }
}