using AppointmentScheduler.AppointmentService.Repositories;
using AppointmentScheduler.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace AppointmentScheduler.AppointmentService.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/suggestions")]
[ApiVersion("1.0")]
public class SuggestionsController : ControllerBase
{
    private readonly IAppointmentRepository _repository;

    public SuggestionsController(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateTime preferredDate)
    {
        var suggestion = await _repository.SuggestBestSlotAsync(preferredDate == default ? DateTime.UtcNow : preferredDate);
        if (suggestion is null)
        {
            return NotFound(new { message = "No free appointment slot found in the next seven days." });
        }

        return Ok(new SuggestionResponse(suggestion.Start, suggestion.End, "Recommended based on availability and your preferred date."));
    }
}
