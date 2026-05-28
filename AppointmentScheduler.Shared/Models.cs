namespace AppointmentScheduler.Shared.Models;

public record AppointmentSlotDto(
    int Id,
    string Title,
    string Description,
    DateTime Start,
    DateTime End,
    bool IsBooked,
    int Capacity,
    int BookedCount);

public record AppointmentSlotCreateDto(
    string Title,
    string Description,
    DateTime Start,
    DateTime End,
    int Capacity);

public record BookingDto(
    int Id,
    int AppointmentSlotId,
    string CustomerName,
    string CustomerEmail,
    string Notes,
    DateTime BookingDateUtc);

public record BookingCreateDto(
    int AppointmentSlotId,
    string CustomerName,
    string CustomerEmail,
    string Notes);

public record AuthRequest(string Username, string Password);
public record AuthResponse(string Token, string TokenType = "Bearer");
public record SuggestionResponse(DateTime SuggestedStart, DateTime SuggestedEnd, string Reason);
public record HealthResponse(string Status, DateTime Timestamp);
