using dimax_front.Core.Entities;
using dimax_front.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace dimax_front.Infrastructure.Context
{
    public class WorkshopDbContext : DbContext
    {
        public WorkshopDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<Vehicle>().HasKey(x => x.Plate);

            modelBuilder.Entity<InstallationHistory>()
                .HasOne(x => x.Vehicle)
                .WithMany(x => x.InstallationHistories)
                .HasForeignKey(x => x.PlateId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InstallationHistory>()
                .HasIndex(i => i.InvoiceNumber)
                .IsUnique();

            modelBuilder.Entity<InstallationHistory>()
                .HasIndex(x => x.TechnicalFileNumber)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => new { u.Id, u.Username, u.Email })
                .IsUnique();
        }

        public DbSet<Vehicle> Vehicules { get; set; }
        public DbSet<InstallationHistory> InstallationHistories { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
