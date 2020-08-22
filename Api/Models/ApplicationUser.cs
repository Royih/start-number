using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Signup.API.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    [CollectionName("Users")]
    public class ApplicationUser : MongoIdentityUser
    {
        public string TenantId { get; set; }

        public string TenantName { get; set; }

        public string FullName { get; set; }
        
        public string Culture { get; set; }

    }
}