using AppointmentScheduler.BookingService.Data;
using AppointmentScheduler.BookingService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppointmentScheduler.BookingService.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly BookingContext _context;

    public BookingRepository(BookingContext context)
    {
        _context = context;
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        return await _context.Bookings.OrderByDescending(x => x.BookingDateUtc).ToListAsync();
    }

    public async Task<List<Booking>> GetBySlotIdAsync(int slotId)
    {
        return await _context.Bookings.Where(x => x.AppointmentSlotId == slotId).OrderByDescending(x => x.BookingDateUtc).ToListAsync();
    }

    public async Task<Booking> AddAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        return booking;
    }
}
