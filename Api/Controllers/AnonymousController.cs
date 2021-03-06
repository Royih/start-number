﻿
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

        [HttpGet("{action}/{key}")]
        public async Task<IActionResult> GetEventData(string key)
        {
            return Ok(await Mediator.Send(new GetEventDataQuery { Key = key }));
        }

        [HttpPost("{action}")]
        public async Task<IActionResult> SignUp(SignUpCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("downloadStartNumber/{signupId}")]
        public async Task<IActionResult> ListSignups(string signupId)
        {
            return await Mediator.Send(new GetStartNumberPdfBySignupQuery
            {
                SignupId = signupId
            });
        }
    }
}
