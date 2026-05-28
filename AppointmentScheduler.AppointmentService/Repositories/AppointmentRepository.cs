using AppointmentScheduler.AppointmentService.Data;
using AppointmentScheduler.AppointmentService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppointmentScheduler.AppointmentService.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppointmentContext _context;

    public AppointmentRepository(AppointmentContext context)
    {
        _context = context;
    }

    public async Task<List<AppointmentSlot>> GetAllAsync()
    {
        return await _context.AppointmentSlots.OrderBy(x => x.Start).ToListAsync();
    }

    public async Task<AppointmentSlot?> GetByIdAsync(int id)
    {
        return await _context.AppointmentSlots.FindAsync(id);
    }

    public async Task<AppointmentSlot> AddAsync(AppointmentSlot slot)
    {
        _context.AppointmentSlots.Add(slot);
        await _context.SaveChangesAsync();
        return slot;
    }

    public async Task UpdateAsync(AppointmentSlot slot)
    {
        _context.AppointmentSlots.Update(slot);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is null)
        {
            return;
        }

        _context.AppointmentSlots.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<List<AppointmentSlot>> GetAvailableAsync(DateTime from, DateTime to)
    {
        return await _context.AppointmentSlots
            .Where(x => x.Start >= from && x.End <= to && x.BookedCount < x.Capacity)
            .OrderBy(x => x.Start)
            .ToListAsync();
    }

    public async Task<AppointmentSlot?> SuggestBestSlotAsync(DateTime preferredDate)
    {
        var windowStart = preferredDate.Date;
        var windowEnd = windowStart.AddDays(7);

        return await _context.AppointmentSlots
            .Where(x => x.BookedCount < x.Capacity && x.Start >= windowStart && x.End <= windowEnd)
            .OrderBy(x => x.Start)
            .FirstOrDefaultAsync();
    }
}
