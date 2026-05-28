import './Calendar.css';

export function Calendar({ slots, bookings }) {
  return (
    <section className="calendar-panel">
      <h2>Appointment Calendar</h2>
      <div className="calendar-grid">
        {slots.length === 0 ? (
          <p>No appointment slots available.</p>
        ) : (
          slots.map(slot => (
            <article key={slot.id} className={`calendar-card ${slot.isBooked ? 'booked' : 'available'}`}>
              <h3>{slot.title}</h3>
              <p>{new Date(slot.start).toLocaleString()} - {new Date(slot.end).toLocaleString()}</p>
              <p>{slot.description}</p>
              <p>Capacity: {slot.capacity} / Booked: {slot.bookedCount}</p>
              <p>Status: {slot.isBooked ? 'Booked' : 'Open'}</p>
            </article>
          ))
        )}
      </div>
      <div className="booking-summary">
        <h3>Recent Bookings</h3>
        {bookings.length === 0 ? (
          <p>No bookings yet.</p>
        ) : (
          <ul>
            {bookings.slice(0, 5).map(booking => (
              <li key={booking.id}>
                {booking.customerName} reserved slot #{booking.appointmentSlotId} on {new Date(booking.bookingDateUtc).toLocaleString()}
              </li>
            ))}
          </ul>
        )}
      </div>
    </section>
  );
}
