namespace AppointmentScheduler.BookingService.Entities;

public class Booking
{
    public int Id { get; set; }
    public int AppointmentSlotId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string CustomerEmail { get; set; } = null!;
    public string Notes { get; set; } = string.Empty;
    public DateTime BookingDateUtc { get; set; } = DateTime.UtcNow;
}
