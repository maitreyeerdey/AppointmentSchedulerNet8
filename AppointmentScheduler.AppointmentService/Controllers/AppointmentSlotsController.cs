using AppointmentScheduler.AppointmentService.Entities;
using AppointmentScheduler.AppointmentService.Repositories;
using AppointmentScheduler.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentScheduler.AppointmentService.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/slots")]
[ApiVersion("1.0")]
public class AppointmentSlotsController : ControllerBase
{
    private readonly IAppointmentRepository _repository;

    public AppointmentSlotsController(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var slots = await _repository.GetAllAsync();
        return Ok(slots.Select(Map));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var slot = await _repository.GetByIdAsync(id);
        return slot is null ? NotFound() : Ok(Map(slot));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] AppointmentSlotCreateDto request)
    {
        var slot = new AppointmentSlot
        {
            Title = request.Title,
            Description = request.Description,
            Start = request.Start,
            End = request.End,
            Capacity = request.Capacity,
            BookedCount = 0
        };

        var created = await _repository.AddAsync(slot);
        return CreatedAtAction(nameof(GetById), new { id = created.Id, version = "1.0" }, Map(created));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] AppointmentSlotCreateDto request)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Title = request.Title;
        existing.Description = request.Description;
        existing.Start = request.Start;
        existing.End = request.End;
        existing.Capacity = request.Capacity;

        await _repository.UpdateAsync(existing);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/book")]
    public async Task<IActionResult> BookSlot(int id)
    {
        var slot = await _repository.GetByIdAsync(id);
        if (slot is null)
        {
            return NotFound();
        }

        if (slot.IsBooked)
        {
            return BadRequest(new { message = "This slot is fully booked." });
        }

        slot.BookOne();
        await _repository.UpdateAsync(slot);
        return NoContent();
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable([FromQuery] DateTime? start, [FromQuery] DateTime? end)
    {
        var from = start ?? DateTime.UtcNow;
        var to = end ?? from.AddDays(7);
        var slots = await _repository.GetAvailableAsync(from, to);
        return Ok(slots.Select(Map));
    }

    private static AppointmentSlotDto Map(AppointmentSlot slot)
    {
        return new AppointmentSlotDto(
            slot.Id,
            slot.Title,
            slot.Description,
            slot.Start,
            slot.End,
            slot.IsBooked,
            slot.Capacity,
            slot.BookedCount);
    }
}
