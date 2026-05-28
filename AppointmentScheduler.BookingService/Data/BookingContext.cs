using AppointmentScheduler.BookingService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppointmentScheduler.BookingService.Data;

public class BookingContext : DbContext
{
    public BookingContext(DbContextOptions<BookingContext> options) : base(options)
    {
    }

    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CustomerName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.CustomerEmail).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Notes).HasMaxLength(1000);
        });

        base.OnModelCreating(modelBuilder);
    }
}
