using AppointmentScheduler.AppointmentService.Controllers;
using AppointmentScheduler.AppointmentService.Entities;
using AppointmentScheduler.AppointmentService.Repositories;
using AppointmentScheduler.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppointmentScheduler.Tests;

public class AppointmentSlotsControllerTests
{
    [Fact]
    public async Task GetById_ReturnsNotFound_WhenSlotDoesNotExist()
    {
        var repository = new FakeAppointmentRepository { GetByIdResult = null };
        var controller = new AppointmentSlotsController(repository);

        var result = await controller.GetById(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WithMappedAppointmentSlot()
    {
        var slot = new AppointmentSlot
        {
            Id = 10,
            Title = "Consultation",
            Description = "Discuss project scope",
            Start = DateTime.UtcNow.AddDays(2),
            End = DateTime.UtcNow.AddDays(2).AddHours(1),
            Capacity = 3,
            BookedCount = 0
        };

        var repository = new FakeAppointmentRepository { AddResult = slot };
        var controller = new AppointmentSlotsController(repository);

        var request = new AppointmentSlotCreateDto(slot.Title, slot.Description, slot.Start, slot.End, slot.Capacity);
        var result = await controller.Create(request);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var dto = Assert.IsType<AppointmentSlotDto>(created.Value!);

        Assert.Equal(slot.Id, dto.Id);
        Assert.Equal(slot.Title, dto.Title);
        Assert.Equal(slot.Description, dto.Description);
        Assert.Equal(slot.Capacity, dto.Capacity);
        Assert.Equal("GetById", created.ActionName);
    }

    [Fact]
    public async Task BookSlot_ReturnsBadRequest_WhenSlotAlreadyBooked()
    {
        var slot = new AppointmentSlot
        {
            Id = 5,
            Capacity = 1,
            BookedCount = 1
        };

        var repository = new FakeAppointmentRepository { GetByIdResult = slot };
        var controller = new AppointmentSlotsController(repository);

        var result = await controller.BookSlot(5);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequest.Value);
        Assert.Contains("fully booked", badRequest.Value!.ToString() ?? string.Empty);
    }

    [Fact]
    public async Task BookSlot_ReturnsNoContent_WhenSlotBookedSuccessfully()
    {
        var slot = new AppointmentSlot
        {
            Id = 5,
            Capacity = 2,
            BookedCount = 0
        };

        var repository = new FakeAppointmentRepository { GetByIdResult = slot };
        var controller = new AppointmentSlotsController(repository);

        var result = await controller.BookSlot(5);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(1, slot.BookedCount);
        Assert.Same(slot, repository.UpdatedSlot);
    }

    [Fact]
    public async Task GetAvailable_ReturnsOk_WithAvailableAppointmentSlots()
    {
        var slot = new AppointmentSlot
        {
            Id = 2,
            Title = "Review",
            Description = "Review meeting",
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(1).AddHours(1),
            Capacity = 4,
            BookedCount = 0
        };

        var repository = new FakeAppointmentRepository
        {
            AvailableResults = new List<AppointmentSlot> { slot }
        };

        var controller = new AppointmentSlotsController(repository);
        var result = await controller.GetAvailable(null, null);

        var ok = Assert.IsType<OkObjectResult>(result);
        var values = Assert.IsAssignableFrom<IEnumerable<AppointmentSlotDto>>(ok.Value!);
        Assert.Single(values);
    }

    private class FakeAppointmentRepository : IAppointmentRepository
    {
        public AppointmentSlot? GetByIdResult { get; set; }
        public AppointmentSlot? AddResult { get; set; }
        public AppointmentSlot? UpdatedSlot { get; private set; }
        public List<AppointmentSlot> AvailableResults { get; set; } = new();

        public Task<List<AppointmentSlot>> GetAllAsync() => Task.FromResult(new List<AppointmentSlot>());
        public Task<AppointmentSlot?> GetByIdAsync(int id) => Task.FromResult(GetByIdResult);
        public Task<AppointmentSlot> AddAsync(AppointmentSlot slot) => Task.FromResult(AddResult ?? slot);
        public Task UpdateAsync(AppointmentSlot slot)
        {
            UpdatedSlot = slot;
            return Task.CompletedTask;
        }
        public Task DeleteAsync(int id) => Task.CompletedTask;
        public Task<List<AppointmentSlot>> GetAvailableAsync(DateTime from, DateTime to) => Task.FromResult(AvailableResults);
        public Task<AppointmentSlot?> SuggestBestSlotAsync(DateTime preferredDate) => Task.FromResult<AppointmentSlot?>(null);
    }
}
