using AppointmentScheduler.AppointmentService.Data;
using AppointmentScheduler.AppointmentService.Entities;
using AppointmentScheduler.AppointmentService.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AppointmentScheduler.Tests.RepositoryTests;

public class AppointmentRepositoryTests
{
    private AppointmentContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppointmentContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new AppointmentContext(options);
    }

    [Fact]
    public async Task AddAsync_AddsAndRetrievesAppointmentSlot()
    {
        using var context = CreateContext("AddAsync_AddsAndRetrievesAppointmentSlot");
        var repo = new AppointmentRepository(context);

        var slot = new AppointmentSlot
        {
            Title = "Test",
            Description = "Desc",
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(1).AddHours(1),
            Capacity = 2
        };

        var created = await repo.AddAsync(slot);

        Assert.NotEqual(0, created.Id);
        var fetched = await repo.GetByIdAsync(created.Id);
        Assert.NotNull(fetched);
        Assert.Equal("Test", fetched!.Title);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesEntity()
    {
        using var context = CreateContext("UpdateAsync_UpdatesEntity");
        var repo = new AppointmentRepository(context);

        var slot = new AppointmentSlot
        {
            Title = "Original",
            Description = "Desc",
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(1).AddHours(1),
            Capacity = 2
        };

        var created = await repo.AddAsync(slot);
        created.Title = "Updated";

        await repo.UpdateAsync(created);

        var fetched = await repo.GetByIdAsync(created.Id);
        Assert.Equal("Updated", fetched!.Title);
    }

    [Fact]
    public async Task DeleteAsync_RemovesEntity()
    {
        using var context = CreateContext("DeleteAsync_RemovesEntity");
        var repo = new AppointmentRepository(context);

        var slot = new AppointmentSlot
        {
            Title = "ToDelete",
            Description = "Desc",
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(1).AddHours(1),
            Capacity = 2
        };

        var created = await repo.AddAsync(slot);
        await repo.DeleteAsync(created.Id);

        var fetched = await repo.GetByIdAsync(created.Id);
        Assert.Null(fetched);
    }

    [Fact]
    public async Task GetAvailableAsync_ReturnsOnlyAvailableSlots()
    {
        using var context = CreateContext("GetAvailableAsync_ReturnsOnlyAvailableSlots");
        var repo = new AppointmentRepository(context);

        var slot1 = new AppointmentSlot { Title = "A", Start = DateTime.UtcNow.AddDays(1), End = DateTime.UtcNow.AddDays(1).AddHours(1), Capacity = 1, BookedCount = 1 };
        var slot2 = new AppointmentSlot { Title = "B", Start = DateTime.UtcNow.AddDays(2), End = DateTime.UtcNow.AddDays(2).AddHours(1), Capacity = 2, BookedCount = 0 };

        await repo.AddAsync(slot1);
        await repo.AddAsync(slot2);

        var from = DateTime.UtcNow;
        var to = from.AddDays(7);
        var avail = await repo.GetAvailableAsync(from, to);

        Assert.Single(avail);
        Assert.Equal("B", avail[0].Title);
    }

    [Fact]
    public async Task SuggestBestSlotAsync_ReturnsFirstAvailableWithinWindow()
    {
        using var context = CreateContext("SuggestBestSlotAsync_ReturnsFirstAvailableWithinWindow");
        var repo = new AppointmentRepository(context);

        var slot = new AppointmentSlot { Title = "Suggested", Start = DateTime.UtcNow.AddDays(3), End = DateTime.UtcNow.AddDays(3).AddHours(1), Capacity = 1, BookedCount = 0 };
        await repo.AddAsync(slot);

        var suggestion = await repo.SuggestBestSlotAsync(DateTime.UtcNow);
        Assert.NotNull(suggestion);
        Assert.Equal("Suggested", suggestion!.Title);
    }
}
