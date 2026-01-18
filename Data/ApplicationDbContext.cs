using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TransportRequestSystem.Models;


namespace TransportRequestSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<StatusHistory> StatusHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Конфигурация для Application
            builder.Entity<Application>(entity =>
            {
                entity.ToTable("Applications");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.Property(e => e.OrganizationUnit)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ResponsiblePerson)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Purpose)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Route)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Notes)
                    .HasMaxLength(1000);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("NOW()");

                entity.HasMany(a => a.StatusHistory)
                     .WithOne(sh => sh.Application)
                     .HasForeignKey(sh => sh.ApplicationId)
                     .OnDelete(DeleteBehavior.Cascade);
            });

            // Конфигурация для StatusHistory
            builder.Entity<StatusHistory>(entity =>
            {
                entity.ToTable("StatusHistory");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.OldStatus)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.NewStatus)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ChangedBy)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Comment)
                    .HasMaxLength(500);

                entity.Property(e => e.ChangedDate)
                    .IsRequired()
                    .HasDefaultValueSql("NOW()");
            });
        }
    }
}