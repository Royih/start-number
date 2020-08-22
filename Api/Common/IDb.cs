
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using Signup.API.Models;

namespace Signup.API.Common
{
    public interface IDb
    {
        IMongoClient Client { get; }
        UserManager<ApplicationUser> UserManager { get; }
        IMongoCollection<Event> Events { get; }
        IMongoCollection<Person> Persons { get; }
        IMongoCollection<SignupRecord> SignupRecords { get; }
        IMongoCollection<Tenant> Tenants { get; }
        IMongoCollection<ApplicationUser> Users { get; }
        IMongoCollection<RefreshToken> RefreshTokens { get; }

    }
}