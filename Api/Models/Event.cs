using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Signup.API.Models
{
    public class Event
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string TenantId { get; set; }

        public EventSignup[] Signups { get; set; }

    }
}