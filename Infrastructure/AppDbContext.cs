using Microsoft.EntityFrameworkCore;
using PetCatalogApp.Domain;
using PetCatalogApp.Data;

namespace PetCatalogApp.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Visit> Visits { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=PetCatalog.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Зв'язок Owner -> Pets
            modelBuilder.Entity<Owner>()
                .HasMany(o => o.Pets)
                .WithOne(p => p.Owner)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Зв'язок Pet -> Visits
            modelBuilder.Entity<Pet>()
                .HasMany(p => p.Visits)
                .WithOne(v => v.Pet)
                .HasForeignKey(v => v.PetId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}