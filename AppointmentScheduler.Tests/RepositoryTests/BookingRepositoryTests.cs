using AppointmentScheduler.BookingService.Data;
using AppointmentScheduler.BookingService.Entities;
using AppointmentScheduler.BookingService.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AppointmentScheduler.Tests.RepositoryTests;

public class BookingRepositoryTests
{
    private BookingContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<BookingContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new BookingContext(options);
    }

    [Fact]
    public async Task AddAsync_AddsBooking()
    {
        using var context = CreateContext("AddAsync_AddsBooking");
        var repo = new BookingRepository(context);

        var booking = new Booking { AppointmentSlotId = 1, CustomerName = "Jane", CustomerEmail = "jane@example.com" };

        var created = await repo.AddAsync(booking);

        Assert.NotEqual(0, created.Id);
        var all = await repo.GetAllAsync();
        Assert.Single(all);
    }

    [Fact]
    public async Task GetBySlotIdAsync_ReturnsMatchingBookings()
    {
        using var context = CreateContext("GetBySlotIdAsync_ReturnsMatchingBookings");
        var repo = new BookingRepository(context);

        var booking1 = new Booking { AppointmentSlotId = 5, CustomerName = "A", CustomerEmail = "a@example.com" };
        var booking2 = new Booking { AppointmentSlotId = 6, CustomerName = "B", CustomerEmail = "b@example.com" };

        await repo.AddAsync(booking1);
        await repo.AddAsync(booking2);

        var slot5 = await repo.GetBySlotIdAsync(5);
        Assert.Single(slot5);
        Assert.Equal(5, slot5[0].AppointmentSlotId);
    }
}
