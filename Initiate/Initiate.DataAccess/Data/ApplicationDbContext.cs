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

            builder.Entity<Country>()
                .HasData(
                    new Country()
                    {
                        CountryId = 1,
                        CountryName = "Canada"
                    }
                );
            builder.Entity<Province>()
                .HasData(
                    new Province()
                    {
                        ProvinceId = 1,
                        ProvinceName = "Ontario"
                    });

            builder.Entity<Address>()
                .HasData(
                    new Address()
                    {
                        AddressId = 1,
                        CountryId = 1,
                        ProvinceId = 1
                    }
                );

            //builder.Entity<Preference>()
            //    .HasData(
            //        new Preference()
            //        {
            //            PreferenceId = 1,
            //            AddressId = 1,
            //            GenerateDate = DateTime.Now,
            //            Language = "English"
            //        }
            //    );
        }

        public DbSet<News> News { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<UserKeyword> UserKeywords { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<NewsKeyword> NewsKeywords { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<Friend> Friends { get; set; }

        

    }
}