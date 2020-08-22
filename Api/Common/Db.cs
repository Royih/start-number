
using System.Security.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Signup.API.Models;

namespace Signup.API.Common
{
    public class Db : IDb
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMongoClient _client;
        private readonly IMongoCollection<Event> _events;
        private readonly IMongoCollection<Person> _persons;
        private readonly IMongoCollection<SignupRecord> _signupRecords;
        private readonly IMongoCollection<Tenant> _tenants;
        private readonly IMongoCollection<ApplicationUser> _users;
        private readonly IMongoCollection<RefreshToken> _refreshTokens;

        public Db(IConfiguration config, UserManager<ApplicationUser> userManager)
        {

            var settings = MongoClientSettings.FromUrl(new MongoUrl(config.GetMongoConnectionString()));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            _client = new MongoClient(settings);
            var database = _client.GetDatabase(config.GetMongoDatabaseName());
            _userManager = userManager;
            _events = database.GetCollection<Event>("events");
            _persons = database.GetCollection<Person>("persons");
            _signupRecords = database.GetCollection<SignupRecord>("signupRecords");
            _tenants = database.GetCollection<Tenant>("tenants");
            _users = database.GetCollection<ApplicationUser>("users");
            _refreshTokens = database.GetCollection<RefreshToken>("refreshTokens");
            _tenants = database.GetCollection<Tenant>("tenants");
            _refreshTokens = database.GetCollection<RefreshToken>("refreshTokens");
        }

        public IMongoClient Client => _client;
        public IMongoCollection<ApplicationUser> Users => _users;
        public UserManager<ApplicationUser> UserManager => _userManager;
        public IMongoCollection<Event> Events => _events;
        public IMongoCollection<Person> Person => _persons;
        public IMongoCollection<RefreshToken> RefreshToken => _refreshTokens;
        public IMongoCollection<SignupRecord> SignupRecords => _signupRecords;
        public IMongoCollection<Tenant> Tenants => _tenants;
        public IMongoCollection<Person> Persons => _persons;
        public IMongoCollection<RefreshToken> RefreshTokens => _refreshTokens;

    }
}