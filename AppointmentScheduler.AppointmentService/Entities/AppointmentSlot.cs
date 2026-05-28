namespace AppointmentScheduler.AppointmentService.Entities;

public class AppointmentSlot
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Capacity { get; set; } = 1;
    public int BookedCount { get; set; }
    public bool IsBooked => BookedCount >= Capacity;

    public void BookOne()
    {
        if (IsBooked)
        {
            throw new InvalidOperationException("The appointment slot is fully booked.");
        }

        BookedCount++;
    }
}
