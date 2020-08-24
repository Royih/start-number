using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Signup.API.Models
{
    public class Tenant
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }
        public string Base64EncodedLogo { get; set; }
        public string Base64EncodedSponsorStripe { get; set; }

        public string CurrentlyActiveEventId { get; set; }



    }
}