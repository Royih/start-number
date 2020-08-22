using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Signup.API.Models
{
    public class RefreshToken
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        public Guid UserId { get; set; }
        // public string Subject { get; set; }
        public string ClientIp { get; set; }
        public DateTime IssuedUtc { get; set; }
        public DateTime ExpiresUtc { get; set; }
        public string ProtectedTicket { get; set; }
        public DateTime? LastUsed { get; set; }
    }
}