using System.Linq;
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

    public class UpdateOwnPasswordCommand : IRequest<CommandResultDto>
    {
        public UserDto User { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class UpdateOwnPasswordCommandHandler : IRequestHandler<UpdateOwnPasswordCommand, CommandResultDto>
    {
        private readonly IUserRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;

        public UpdateOwnPasswordCommandHandler(IUserRepository repo, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _userManager = userManager;
        }

        public async Task<CommandResultDto> Handle(UpdateOwnPasswordCommand command, CancellationToken cancellationToken)
        {
            var user = _repo.GetCurrentUser();
            if (user.Id != command.User.Id)
            {
                throw new SecurityException("User is only allowed to update own profile!");
            }

            var usr = await _userManager.FindByIdAsync(user.Id.ToString());

            var result = await _userManager.ChangePasswordAsync(usr, command.CurrentPassword, command.NewPassword);

            return new CommandResultDto { Success = result.Succeeded, ErrorMessages = result.Errors.Any() ? result.Errors.Select(x => x.Code + ": " + x.Description).ToArray() : null };

        }
    }
}