export function BookingForm({ slots, onSubmit, onSuggest, suggested, selectedSlotId, setSelectedSlotId, formValues, setFormValues, isLoading }) {
  return (
    <section className="form-panel">
      <h2>Book an Appointment</h2>
      <label>
        Appointment slot
        <select value={selectedSlotId} onChange={e => setSelectedSlotId(Number(e.target.value))}>
          <option value={0}>Select a slot</option>
          {slots.filter(slot => !slot.isBooked).map(slot => (
            <option key={slot.id} value={slot.id}>
              {slot.title} — {new Date(slot.start).toLocaleString()}
            </option>
          ))}
        </select>
      </label>
      <label>
        Name
        <input type="text" value={formValues.customerName} onChange={e => setFormValues({ ...formValues, customerName: e.target.value })} />
      </label>
      <label>
        Email
        <input type="email" value={formValues.customerEmail} onChange={e => setFormValues({ ...formValues, customerEmail: e.target.value })} />
      </label>
      <label>
        Notes
        <textarea value={formValues.notes} onChange={e => setFormValues({ ...formValues, notes: e.target.value })} />
      </label>
      <div className="form-actions">
        <button type="button" onClick={onSubmit} disabled={!selectedSlotId || isLoading}>{isLoading ? 'Booking...' : 'Book now'}</button>
        <button type="button" className="secondary" onClick={onSuggest} disabled={isLoading}>{isLoading ? 'Suggesting...' : 'Suggest best time'}</button>
      </div>
      {suggested && (
        <div className="suggestion-box">
          <strong>Suggested slot:</strong> {new Date(suggested.suggestedStart).toLocaleString()} — {new Date(suggested.suggestedEnd).toLocaleString()}
          <p>{suggested.reason}</p>
        </div>
      )}
    </section>
  );
}
