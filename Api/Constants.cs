
using System.Collections.Generic;
using System.Linq;
using Signup.API.Dtos;

namespace Signup.API
{

    public enum Roles
    {
        User,
        Admin
    }
    public static class Constants
    {
        public const string AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789æøåÆØÅ-@. _";
        public const string AppSettingCorsOrigin = "CorsOrigin";
        public const string AppSettingSecretKey = "SecretKey";

        public static IEnumerable<CultureDto> ListCultures
        {
            get
            {
                yield return new CultureDto { Key = "nb-NO", Value = "Norsk" };
                yield return new CultureDto { Key = "en-US", Value = "English" };
            }
        }
        public static CultureDto GetCulture(string key)
        {
            return ListCultures.Single(x => x.Key == key);
        }

    }



}