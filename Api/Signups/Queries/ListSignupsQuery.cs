using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Signup.API.Signups.Dtos;
using Signup.API.Users.Repos;

namespace Signup.API.Signups.Queries
{

    public class ListSignupsQuery : IRequest<IEnumerable<SignUpsForEventDto>>
    {
        public string EventId { get; set; }
    }

    public class ListSignupsQueryValidator : AbstractValidator<ListSignupsQuery>
    {
        public ListSignupsQueryValidator()
        {
            RuleFor(x => x.EventId).NotEmpty();
        }
    }

    public class ListSignupsQueryHandler : IRequestHandler<ListSignupsQuery, IEnumerable<SignUpsForEventDto>>
    {
        private readonly ISignupRepository _repo;

        private readonly IMapper _mapper;

        public ListSignupsQueryHandler(ISignupRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }



        async Task<IEnumerable<SignUpsForEventDto>> IRequestHandler<ListSignupsQuery, IEnumerable<SignUpsForEventDto>>.Handle(ListSignupsQuery request, CancellationToken cancellationToken)
        {
            return await _repo.ListSignups(request.EventId);
        }
    }
}