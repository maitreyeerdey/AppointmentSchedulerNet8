using AppointmentScheduler.Shared.Models;
using FluentValidation;

namespace AppointmentScheduler.BookingService.Validators;

public class BookingCreateDtoValidator : AbstractValidator<BookingCreateDto>
{
    public BookingCreateDtoValidator()
    {
        RuleFor(x => x.AppointmentSlotId).GreaterThan(0);
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CustomerEmail).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
