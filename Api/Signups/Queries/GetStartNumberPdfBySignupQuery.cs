
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Signup.API.Users.Repos;

namespace Signup.API.Signups.Queries
{

    public class GetStartNumberPdfBySignupQuery : IRequest<FileContentResult>
    {
        public string SignupId { get; set; }
    }

    public class GetStartNumberPdfBySignupQueryValidator : AbstractValidator<GetStartNumberPdfBySignupQuery>
    {
        public GetStartNumberPdfBySignupQueryValidator()
        {
            RuleFor(x => x.SignupId).NotEmpty();
        }
    }

    public class GetStartNumberPdfBySignupQueryHandler : IRequestHandler<GetStartNumberPdfBySignupQuery, FileContentResult>
    {
        private readonly IAnonymousRepository _repo;



        public GetStartNumberPdfBySignupQueryHandler(IAnonymousRepository repo)
        {
            _repo = repo;
        }



        async Task<FileContentResult> IRequestHandler<GetStartNumberPdfBySignupQuery, FileContentResult>.Handle(GetStartNumberPdfBySignupQuery request, CancellationToken cancellationToken)
        {
            var signup = await _repo.GetSignup(request.SignupId);
            var pdfAsByteArray = _repo.GetStartNumberPdf(signup);
            return new FileContentResult(pdfAsByteArray, "application/pdf")
            {
                FileDownloadName = signup.GetStartNumberPdfFileName()
            };
        }
    }
}