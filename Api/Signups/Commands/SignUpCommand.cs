using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Signup.API.Dtos;
using Signup.API.Models;
using Signup.API.Users.Repos;

namespace Signup.API.Users.Commands
{

    public class SignUpCommand : IRequest<CommandResultDto>
    {
        public SignUpDto SignUpData { get; set; }
    }

    public class SignUpCommandValidator : AbstractValidator<SignUpCommand>
    {
        public SignUpCommandValidator()
        {
            RuleFor(x => x.SignUpData).NotNull();
            RuleFor(x => x.SignUpData.TenantKey).NotEmpty();
            RuleFor(x => x.SignUpData.FirstName).Length(2, 100);
            RuleFor(x => x.SignUpData.SurName).Length(2, 100);
            RuleFor(x => x.SignUpData.Email).EmailAddress();
        }
    }

    public class SignUpCommandHandler : IRequestHandler<SignUpCommand, CommandResultDto>
    {

        private readonly IAnonymousRepository _repo;

        public SignUpCommandHandler(IAnonymousRepository repo)
        {
            _repo = repo;
        }

        public async Task<CommandResultDto> Handle(SignUpCommand command, CancellationToken cancellationToken)
        {
            return await _repo.SignUp(command.SignUpData);

        }
    }

}