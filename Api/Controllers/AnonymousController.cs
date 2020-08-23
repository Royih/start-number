
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Signup.API.Signups.Queries;
using Signup.API.Users.Commands;

namespace Signup.API.Controllers
{

    [AllowAnonymous]
    public class AnonymousController : SignupControllerBase
    {
        public AnonymousController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("{action}")]
        public async Task<IActionResult> ListEvents()
        {
            return Ok(await Mediator.Send(new ListActiveEventsQuery()));
        }

        [HttpPost("{action}")]
        public async Task<IActionResult> SignUp(SignUpCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
