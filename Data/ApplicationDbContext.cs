using Microsoft.EntityFrameworkCore;
using TransportRequestSystem.Models;

namespace TransportRequestSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Таблицы
        public DbSet<User> Users { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<StatusHistory> StatusHistory { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Конфигурация User
            builder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();

                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);

                // Связи
                entity.HasMany(u => u.CreatedApplications)
                    .WithOne(a => a.CreatedByUser)
                    .HasForeignKey(a => a.CreatedByUserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(u => u.DispatchedApplications)
                    .WithOne(a => a.DispatcherUser)
                    .HasForeignKey(a => a.DispatcherUserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Конфигурация Application
            builder.Entity<Application>(entity =>
            {
                entity.ToTable("Applications");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Number).HasMaxLength(50);
                entity.Property(e => e.Status).IsRequired().HasConversion<string>().HasMaxLength(50);
                entity.Property(e => e.OrganizationUnit).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ResponsiblePerson).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Purpose).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Route).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Passengers).IsRequired();
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("NOW()");

                // Внешние ключи
                entity.Property(e => e.CreatedByUserId).HasColumnType("integer");
                entity.Property(e => e.DispatcherUserId).HasColumnType("integer");

                // Связи
                entity.HasOne(a => a.CreatedByUser)
                    .WithMany(u => u.CreatedApplications)
                    .HasForeignKey(a => a.CreatedByUserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(a => a.DispatcherUser)
                    .WithMany(u => u.DispatchedApplications)
                    .HasForeignKey(a => a.DispatcherUserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(a => a.Driver)
                    .WithMany(d => d.Applications)
                    .HasForeignKey(a => a.DriverId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(a => a.Vehicle)
                    .WithMany(v => v.Applications)
                    .HasForeignKey(a => a.VehicleId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(a => a.StatusHistories)
                    .WithOne(sh => sh.Application)
                    .HasForeignKey(sh => sh.ApplicationId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            // Конфигурация остальных таблиц...
            ConfigureStatusHistoryEntity(builder);
            ConfigureDriverEntity(builder);
            ConfigureVehicleEntity(builder);
        }

        private void ConfigureStatusHistoryEntity(ModelBuilder builder)
        {
            builder.Entity<StatusHistory>(entity =>
            {
                entity.ToTable("StatusHistory");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OldStatus).HasMaxLength(50);
                entity.Property(e => e.NewStatus).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ChangedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Comment).HasMaxLength(500);
                entity.Property(e => e.ChangedDate).IsRequired().HasDefaultValueSql("NOW()");
            });
        }

        private void ConfigureDriverEntity(ModelBuilder builder)
        {
            builder.Entity<Driver>(entity =>
            {
                entity.ToTable("Drivers");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.PersonnelNumber).IsUnique();
                entity.Property(e => e.PersonnelNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.LicenseCategory).HasMaxLength(20);
                entity.Property(e => e.Notes).HasMaxLength(500);
            });
        }

        private void ConfigureVehicleEntity(ModelBuilder builder)
        {
            builder.Entity<Vehicle>(entity =>
            {
                entity.ToTable("Vehicles");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.PlateNumber).IsUnique();
                entity.Property(e => e.PlateNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Brand).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Model).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Color).HasMaxLength(30);
                entity.Property(e => e.Notes).HasMaxLength(500);
            });
        }
    }
}