using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Initiate.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<User> // Inherits from IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // This configures the schema needed for the identity framework
            // Further model configurations go here
        }

        public DbSet<News> News { get; set; }
        // Users DbSet is not needed since it's included in IdentityDbContext
    }
}