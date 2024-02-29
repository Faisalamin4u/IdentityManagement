using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityManagement.API.Data
{
    public class IdentityManagementDbContext:IdentityDbContext
    {
        public IdentityManagementDbContext(DbContextOptions<IdentityManagementDbContext> options) : base(options)

        {

        }
    }
}
