

using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Signup.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors]
    public class SignupControllerBase : ControllerBase
    {

        protected readonly IMediator Mediator;

        public SignupControllerBase(IMediator mediator)
        {
            Mediator = mediator;
        }

    }
}