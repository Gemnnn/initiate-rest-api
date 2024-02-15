using Initiate.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Initiate.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<User> // Inherits from IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Friend>()
                .HasOne(f => f.Requester)
                .WithMany(u => u.RequestedFriends)
                .HasForeignKey(f => f.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Friend>()
                .HasOne(f => f.Receiver)
                .WithMany(u => u.ReceivedFriends)
                .HasForeignKey(f => f.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<News> News { get; set; }
        public DbSet<UserKeyword> UserKeywords { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<NewsKeyword> NewsKeywords { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<Friend> Friends { get; set; }

        

    }
}