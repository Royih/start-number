
using System.IO;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Signup.API.Signups.Queries;

namespace Signup.API.Controllers
{

    [Authorize(Roles = "User, Admin")]
    public class SignupController : SignupControllerBase
    {
        public SignupController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("listSignups/{eventId}")]
        public async Task<IActionResult> ListSignups(string eventId)
        {
            return Ok(await Mediator.Send(new ListSignupsQuery { EventId = eventId }));
        }

        [HttpGet("downloadStartNumber/{eventId}/{personId}")]
        public async Task<IActionResult> ListSignups(string eventId, string personId)
        {
            return await Mediator.Send(new GetStartNumberPdfQuery
            {
                EventId = eventId,
                PersonId = personId
            });

        }

    }
}
