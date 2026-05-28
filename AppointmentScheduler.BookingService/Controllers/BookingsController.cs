using AppointmentScheduler.BookingService.Entities;
using AppointmentScheduler.BookingService.Repositories;
using AppointmentScheduler.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentScheduler.BookingService.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/bookings")]
[ApiVersion("1.0")]
public class BookingsController : ControllerBase
{
    private readonly IBookingRepository _repository;
    private readonly IHttpClientFactory _clientFactory;

    public BookingsController(IBookingRepository repository, IHttpClientFactory clientFactory)
    {
        _repository = repository;
        _clientFactory = clientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var bookings = await _repository.GetAllAsync();
        return Ok(bookings.Select(b => new BookingDto(b.Id, b.AppointmentSlotId, b.CustomerName, b.CustomerEmail, b.Notes, b.BookingDateUtc)));
    }

    [HttpGet("by-slot/{slotId}")]
    public async Task<IActionResult> GetBySlot(int slotId)
    {
        var bookings = await _repository.GetBySlotIdAsync(slotId);
        return Ok(bookings.Select(b => new BookingDto(b.Id, b.AppointmentSlotId, b.CustomerName, b.CustomerEmail, b.Notes, b.BookingDateUtc)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BookingCreateDto request)
    {
        var client = _clientFactory.CreateClient("AppointmentService");
        var slotResponse = await client.GetAsync($"api/v1/slots/{request.AppointmentSlotId}");
        if (!slotResponse.IsSuccessStatusCode)
        {
            return BadRequest(new { message = "Selected appointment slot does not exist." });
        }

        var bookResponse = await client.PostAsync($"api/v1/slots/{request.AppointmentSlotId}/book", null);
        if (!bookResponse.IsSuccessStatusCode)
        {
            return BadRequest(new { message = "Could not reserve the appointment slot. It may already be booked." });
        }

        var booking = new Booking
        {
            AppointmentSlotId = request.AppointmentSlotId,
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            Notes = request.Notes,
            BookingDateUtc = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(booking);
        return CreatedAtAction(nameof(GetBySlot), new { slotId = created.AppointmentSlotId, version = "1.0" }, new BookingDto(created.Id, created.AppointmentSlotId, created.CustomerName, created.CustomerEmail, created.Notes, created.BookingDateUtc));
    }
}
