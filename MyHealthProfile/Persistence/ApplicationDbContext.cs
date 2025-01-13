using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyHealthProfile.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MyHealthProfile.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<Patient>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<UserAllergy> UserAllergies { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<UserAllergy>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserAllergies)
                .HasForeignKey(ua => ua.UserId);

            builder.Entity<UserAllergy>()
                .HasOne(ua => ua.Allergy)
                .WithMany(a => a.UserAllergies)
                .HasForeignKey(ua => ua.AllergyId);

            builder.Entity<Allergy>().HasData(
       new Allergy { Id = 1, Name = "Milk" },
       new Allergy { Id = 2, Name = "Egg" },
       new Allergy { Id = 3, Name = "Fish" },
       new Allergy { Id = 4, Name = "Peanuts" },
       new Allergy { Id = 5, Name = "Wheat" }
   );
        }
    }
}
