
namespace Signup.API.Dtos
{
    public class CommandResultDto<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public string[] ErrorMessages { get; set; }
        public string[] Messages { get; set; }
    }

}