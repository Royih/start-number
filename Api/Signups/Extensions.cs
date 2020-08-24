using Signup.API.Models;

namespace Signup.API.Signups
{

    public static class SignupExtensions
    {
        public static string GetStartNumberPdfFileName(this SignupRecord signupRecord)
        {
            return $"Start_Number_{signupRecord.ActualStartNumber}.pdf";
        }
    }

}