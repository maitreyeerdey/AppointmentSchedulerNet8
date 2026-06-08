using AppointmentScheduler.AppointmentService.Controllers;
using AppointmentScheduler.AppointmentService.Entities;
using AppointmentScheduler.AppointmentService.Repositories;
using AppointmentScheduler.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppointmentScheduler.Tests;

public class SuggestionsControllerTests
{
    [Fact]
    public async Task Get_ReturnsOk_WhenSuggestionIsAvailable()
    {
        var slot = new AppointmentSlot
        {
            Id = 1,
            Title = "Strategy Session",
            Description = "Plan next steps",
            Start = DateTime.UtcNow.AddDays(2),
            End = DateTime.UtcNow.AddDays(2).AddHours(1),
            Capacity = 5,
            BookedCount = 0
        };

        var repository = new FakeAppointmentRepository { SuggestBestSlotResult = slot };
        var controller = new SuggestionsController(repository);

        var result = await controller.Get(DateTime.UtcNow);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<SuggestionResponse>(ok.Value!);

        Assert.Equal(slot.Start, response.SuggestedStart);
        Assert.Equal(slot.End, response.SuggestedEnd);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenNoSuggestionExists()
    {
        var repository = new FakeAppointmentRepository();
        var controller = new SuggestionsController(repository);

        var result = await controller.Get(DateTime.UtcNow);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFound.Value);
        Assert.Contains("No free appointment slot found in the next seven days.", notFound.Value!.ToString() ?? string.Empty);
    }

    private class FakeAppointmentRepository : IAppointmentRepository
    {
        public AppointmentSlot? SuggestBestSlotResult { get; set; }

        public Task<List<AppointmentSlot>> GetAllAsync() => Task.FromResult(new List<AppointmentSlot>());
        public Task<AppointmentSlot?> GetByIdAsync(int id) => Task.FromResult<AppointmentSlot?>(null);
        public Task<AppointmentSlot> AddAsync(AppointmentSlot slot) => Task.FromResult(slot);
        public Task UpdateAsync(AppointmentSlot slot) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
        public Task<List<AppointmentSlot>> GetAvailableAsync(DateTime from, DateTime to) => Task.FromResult(new List<AppointmentSlot>());
        public Task<AppointmentSlot?> SuggestBestSlotAsync(DateTime preferredDate) => Task.FromResult(SuggestBestSlotResult);
    }
}
