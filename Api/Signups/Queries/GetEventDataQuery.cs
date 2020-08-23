using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Signup.API.Signups.Dtos;
using Signup.API.Users.Repos;

namespace Signup.API.Signups.Queries
{

    public class GetEventDataQuery : IRequest<EventDataDto>
    {
        public string Key { get; set; }
    }

    public class GetEventDataQueryHandler : IRequestHandler<GetEventDataQuery, EventDataDto>
    {
        private readonly IAnonymousRepository _repo;

        private readonly IMapper _mapper;

        public GetEventDataQueryHandler(IAnonymousRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }



        async Task<EventDataDto> IRequestHandler<GetEventDataQuery, EventDataDto>.Handle(GetEventDataQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetEventData(request.Key);

        }
    }
}