using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Signup.API.Signups.Dtos;
using Signup.API.Users.Repos;

namespace Signup.API.Signups.Queries
{

    public class ListActiveEventsQuery : IRequest<IEnumerable<ActiveEventDto>>
    {

    }

    public class ListActiveEventsQueryHandler : IRequestHandler<ListActiveEventsQuery, IEnumerable<ActiveEventDto>>
    {
        private readonly IAnonymousRepository _repo;

        private readonly IMapper _mapper;

        public ListActiveEventsQueryHandler(IAnonymousRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }



        async Task<IEnumerable<ActiveEventDto>> IRequestHandler<ListActiveEventsQuery, IEnumerable<ActiveEventDto>>.Handle(ListActiveEventsQuery request, CancellationToken cancellationToken)
        {
            return await _repo.ListActiveEvents();

        }
    }
}