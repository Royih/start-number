using System;
using Microsoft.Extensions.Configuration;

namespace Signup.API
{
    public static class Extensions
    {
        public static string GetMongoConnectionString(this IConfiguration config)
        {
            return config.GetValue<string>("ConnectionString");
        }

        public static string GetMongoDatabaseName(this IConfiguration config)
        {
            return config.GetValue<string>("Database");
        }

        public static Guid? AsGuid(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            if (Guid.TryParse(input, out var guid))
            {
                return guid;
            }
            return null;
        }
    }

}