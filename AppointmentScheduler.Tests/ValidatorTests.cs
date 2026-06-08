using AppointmentScheduler.AppointmentService.Validators;
using AppointmentScheduler.BookingService.Validators;
using AppointmentScheduler.Shared.Models;
using Xunit;

namespace AppointmentScheduler.Tests;

public class AppointmentSlotCreateDtoValidatorTests
{
    [Fact]
    public void Validate_ReturnsSuccess_ForValidAppointmentSlotCreateDto()
    {
        var validator = new AppointmentSlotCreateDtoValidator();
        var dto = new AppointmentSlotCreateDto(
            "Consultation",
            "Review project details",
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(1),
            5);

        var result = validator.Validate(dto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ReturnsFailure_WhenStartIsAfterEnd()
    {
        var validator = new AppointmentSlotCreateDtoValidator();
        var dto = new AppointmentSlotCreateDto(
            "Consultation",
            "Review project details",
            DateTime.UtcNow.AddHours(2),
            DateTime.UtcNow.AddHours(1),
            5);

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(dto.Start));
    }

    [Fact]
    public void Validate_ReturnsFailure_WhenCapacityIsZero()
    {
        var validator = new AppointmentSlotCreateDtoValidator();
        var dto = new AppointmentSlotCreateDto(
            "Consultation",
            "Review project details",
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(1),
            0);

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(dto.Capacity));
    }
}

public class BookingCreateDtoValidatorTests
{
    [Fact]
    public void Validate_ReturnsSuccess_ForValidBookingCreateDto()
    {
        var validator = new BookingCreateDtoValidator();
        var dto = new BookingCreateDto(1, "Jane Doe", "jane@example.com", "Please send details.");

        var result = validator.Validate(dto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ReturnsFailure_ForInvalidEmailAndEmptyCustomerName()
    {
        var validator = new BookingCreateDtoValidator();
        var dto = new BookingCreateDto(1, string.Empty, "invalid-email", string.Empty);

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(dto.CustomerName));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(dto.CustomerEmail));
    }
}
