using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace APIlady.Models
{
    public class LadyContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Department> Department { get; set; }
        public LadyContext(DbContextOptions<LadyContext> options) :base(options) 
        {

        }
    }
}
