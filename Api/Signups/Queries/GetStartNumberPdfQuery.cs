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

    public class GetStartNumberPdfQuery : IRequest<byte[]>
    {
        public string EventId { get; set; }
        public string PersonId { get; set; }
    }

    public class GetStartNumberPdfQueryValidator : AbstractValidator<GetStartNumberPdfQuery>
    {
        public GetStartNumberPdfQueryValidator()
        {
            RuleFor(x => x.EventId).NotEmpty();
        }
    }

    public class GetStartNumberPdfQueryHandler : IRequestHandler<GetStartNumberPdfQuery, byte[]>
    {
        private readonly ISignupRepository _repo;

        private readonly IMapper _mapper;

        public GetStartNumberPdfQueryHandler(ISignupRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }



        async Task<byte[]> IRequestHandler<GetStartNumberPdfQuery, byte[]>.Handle(GetStartNumberPdfQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetStartNumberPdf(request.EventId, request.PersonId);
        }
    }
}