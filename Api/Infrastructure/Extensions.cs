using System;
using Microsoft.Extensions.Configuration;

namespace Signup.API.Infrastructure
{

    public static class Extensions
    {
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

        public static string GetMongoConnectionString(this IConfiguration config)
        {
            return config.GetValue<string>("ConnectionString");
        }

        public static string GetMongoDatabaseName(this IConfiguration config)
        {
            return config.GetValue<string>("Database");
        }
    }
}