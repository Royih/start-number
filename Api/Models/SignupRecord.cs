using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Signup.API.Models
{
    public class SignupRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string TenantId { get; set; }
        public DateTime SignupUTC { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public bool AllowUsToContactPersonByEmail { get; set; }
        public bool PreviouslyParticipated { get; set; }
        public string IPAddress { get; set; }
        public string PersonId { get; set; }

    }
}