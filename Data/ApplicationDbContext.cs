using FitnessPro.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessPro.Data
{
    public class ApplicationDbContext : IdentityDbContext<FitnessUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Trainer> Trainers { get; set; } = default!;
        public virtual DbSet<Client> Clients { get; set; } = default!;
        public virtual DbSet<FitnessClass> FitnessClasses { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FitnessUser>()
                .HasMany(fu => fu.Trainers)
                .WithOne(t => t.FitnessUser)
                .HasForeignKey(t => t.FitnessUserId)
                .OnDelete(DeleteBehavior.Restrict); // Choose the appropriate delete behavior

            modelBuilder.Entity<FitnessUser>()
                .HasMany(fu => fu.FitnessClasses)
                .WithOne(fc => fc.FitnessUser)
                .HasForeignKey(fc => fc.FitnessUserId)
                .OnDelete(DeleteBehavior.Restrict); // For FitnessClass relationship
        }

    }
}