using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading;
using Signup.API.Models;
using Signup.API.Users.Repos;
using Signup.API.Dtos;

namespace Signup.API.Users.Commands
{

    public class DeleteUserCommand : IRequest<CommandResultDto>
    {
        public UserDto User { get; set; }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, CommandResultDto>
    {
        private readonly IUserRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteUserCommandHandler(IUserRepository repo, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _userManager = userManager;
        }

        public async Task<CommandResultDto> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            var userNameAndEmail = command.User.UserName;
            var user = await _userManager.FindByNameAsync(command.User.UserName);
            var roles = await _userManager.GetRolesAsync(user);
            foreach(var role in roles)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }
            var t = await _userManager.DeleteAsync(user);
            if (t.Succeeded)
            {
                return new CommandResultDto() { Success = true };
            }
            else
            {
                return new CommandResultDto { Success = false, ErrorMessages = t.Errors.Select(x => x.Description).ToArray() };
            }
        }
    }
}
