using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Signup.API.Models
{
    public class Person
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string TenantId { get; set; }

        public string FirstName { get; set; }
        public string SurName { get; set; }

        public string EMail { get; set; }

        public bool AllowUsToContactPersonByEmail { get; set; }

    }
}