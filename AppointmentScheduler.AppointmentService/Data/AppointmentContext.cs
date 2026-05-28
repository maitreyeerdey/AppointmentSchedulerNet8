using AppointmentScheduler.AppointmentService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppointmentScheduler.AppointmentService.Data;

public class AppointmentContext : DbContext
{
    public AppointmentContext(DbContextOptions<AppointmentContext> options) : base(options)
    {
    }

    public DbSet<AppointmentSlot> AppointmentSlots => Set<AppointmentSlot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppointmentSlot>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(1000);
            builder.Property(x => x.Start).IsRequired();
            builder.Property(x => x.End).IsRequired();
            builder.Property(x => x.Capacity).IsRequired();
            builder.Property(x => x.BookedCount).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
