using Microsoft.AspNetCore.Identity;

namespace IdentityManagement.API.Model.Entities
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
    }
}
