using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Signup.API.Dtos;
using Signup.API.Signups.Dtos;
using Signup.API.Users.Repos;

namespace Signup.API.Users.Commands
{

    public class SignUpCommand : IRequest<CommandResultDto<string>>
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

    public class SignUpCommandHandler : IRequestHandler<SignUpCommand, CommandResultDto<string>>
    {

        private readonly IAnonymousRepository _repo;

        public SignUpCommandHandler(IAnonymousRepository repo)
        {
            _repo = repo;
        }

        public async Task<CommandResultDto<string>> Handle(SignUpCommand command, CancellationToken cancellationToken)
        {
            return await _repo.SignUp(command.SignUpData);

        }
    }

}