using AppointmentScheduler.BookingService.Entities;

namespace AppointmentScheduler.BookingService.Repositories;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllAsync();
    Task<List<Booking>> GetBySlotIdAsync(int slotId);
    Task<Booking> AddAsync(Booking booking);
}
