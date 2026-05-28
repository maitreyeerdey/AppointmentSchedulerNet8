using AppointmentScheduler.Shared.Models;
using Xunit;

namespace AppointmentScheduler.Tests;

public class SharedModelTests
{
    [Fact]
    public void BookingCreateDto_ShouldCaptureCustomerInformation()
    {
        var createDto = new BookingCreateDto(5, "Jane Doe", "jane@example.com", "Prefer a morning slot");

        Assert.Equal(5, createDto.AppointmentSlotId);
        Assert.Equal("Jane Doe", createDto.CustomerName);
        Assert.Equal("jane@example.com", createDto.CustomerEmail);
        Assert.Equal("Prefer a morning slot", createDto.Notes);
    }
}
