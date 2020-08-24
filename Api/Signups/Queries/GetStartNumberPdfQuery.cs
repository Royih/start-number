
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Signup.API.Users.Repos;

namespace Signup.API.Signups.Queries
{

    public class GetStartNumberPdfQuery : IRequest<FileContentResult>
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

    public class GetStartNumberPdfQueryHandler : IRequestHandler<GetStartNumberPdfQuery, FileContentResult>
    {
        private readonly IAnonymousRepository _repo;



        public GetStartNumberPdfQueryHandler(IAnonymousRepository repo)
        {
            _repo = repo;
        }



        async Task<FileContentResult> IRequestHandler<GetStartNumberPdfQuery, FileContentResult>.Handle(GetStartNumberPdfQuery request, CancellationToken cancellationToken)
        {
            var data = await _repo.GetStartNumberPdf(request.EventId, request.PersonId);

            return new FileContentResult(data, "application/pdf")
            {
                FileDownloadName = "file_name_here.pdf"
            };
        }
    }
}