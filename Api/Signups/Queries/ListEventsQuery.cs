using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Signup.API.Dtos;
using Signup.API.Users.Repos;

namespace Signup.API.Signups.Queries
{

    public class ListEventsQuery : IRequest<IEnumerable<ActiveEventDto>>
    {

    }

    public class ListEventsQueryHandler : IRequestHandler<ListEventsQuery, IEnumerable<ActiveEventDto>>
    {
        private readonly IAnonymousRepository _repo;

        private readonly IMapper _mapper;

        public ListEventsQueryHandler(IAnonymousRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }



        async Task<IEnumerable<ActiveEventDto>> IRequestHandler<ListEventsQuery, IEnumerable<ActiveEventDto>>.Handle(ListEventsQuery request, CancellationToken cancellationToken)
        {
            return await _repo.ListActiveEvents();

        }
    }
}