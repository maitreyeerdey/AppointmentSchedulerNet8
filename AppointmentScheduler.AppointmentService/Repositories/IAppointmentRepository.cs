using AppointmentScheduler.AppointmentService.Entities;

namespace AppointmentScheduler.AppointmentService.Repositories;

public interface IAppointmentRepository
{
    Task<List<AppointmentSlot>> GetAllAsync();
    Task<AppointmentSlot?> GetByIdAsync(int id);
    Task<AppointmentSlot> AddAsync(AppointmentSlot slot);
    Task UpdateAsync(AppointmentSlot slot);
    Task DeleteAsync(int id);
    Task<List<AppointmentSlot>> GetAvailableAsync(DateTime from, DateTime to);
    Task<AppointmentSlot?> SuggestBestSlotAsync(DateTime preferredDate);
}
