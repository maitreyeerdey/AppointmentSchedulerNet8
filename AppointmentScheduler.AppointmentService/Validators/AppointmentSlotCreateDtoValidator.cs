using AppointmentScheduler.Shared.Models;
using FluentValidation;

namespace AppointmentScheduler.AppointmentService.Validators;

public class AppointmentSlotCreateDtoValidator : AbstractValidator<AppointmentSlotCreateDto>
{
    public AppointmentSlotCreateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.Start)
            .LessThan(x => x.End).WithMessage("Start must be before end time.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than zero.");
    }
}
