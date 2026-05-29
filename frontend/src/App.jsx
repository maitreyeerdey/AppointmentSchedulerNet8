import { useEffect, useState } from 'react';
import { getSlots, getBookings, createBooking, createSlot, getSuggestions, login } from './services/api';
import { BookingForm } from './components/BookingForm';
import { Calendar } from './components/Calendar';
import './App.css';

function App() {
  const [activeMenu, setActiveMenu] = useState('booking'); // 'booking' or 'admin'
  const [slots, setSlots] = useState([]);
  const [bookings, setBookings] = useState([]);
  const [selectedSlotId, setSelectedSlotId] = useState(0);
  const [formValues, setFormValues] = useState({ customerName: '', customerEmail: '', notes: '' });
  const [suggestion, setSuggestion] = useState(null);
  const [token, setToken] = useState(() => localStorage.getItem('token') ?? '');
  const [adminUsername, setAdminUsername] = useState(() => localStorage.getItem('adminUsername') ?? '');
  const [adminForm, setAdminForm] = useState({ title: '', description: '', start: '', end: '', capacity: 1 });
  const [isLoading, setIsLoading] = useState(false);
  const [message, setMessage] = useState('');
  const [errorInfo, setErrorInfo] = useState('');
  const [authForm, setAuthForm] = useState({ username: 'admin', password: 'password' });
  const isAdminAuthenticated = Boolean(token && adminUsername);

  useEffect(() => {
    loadData();
  }, []);

  async function loadData() {
    setIsLoading(true);
    try {
      const slotsData = await getSlots();
      const bookingsData = await getBookings();
      setSlots(slotsData);
      setBookings(bookingsData);
      setMessage('');
      setErrorInfo('');
    } catch (error) {
      const messageText = error?.message || 'Failed to load data.';
      setMessage(messageText);
      setErrorInfo(typeof error === 'object' ? JSON.stringify(error, Object.getOwnPropertyNames(error), 2) : '');
    } finally {
      setIsLoading(false);
    }
  }

  async function handleBook() {
    if (!selectedSlotId) {
      setMessage('Please choose an appointment slot.');
      return;
    }
    setIsLoading(true);
    try {
      await createBooking({ appointmentSlotId: selectedSlotId, ...formValues });
      setMessage('Booking created successfully.');
      setErrorInfo('');
      setFormValues({ customerName: '', customerEmail: '', notes: '' });
      setSelectedSlotId(0);
      await loadData();
    } catch (error) {
      const messageText = error?.message || 'Failed to create booking.';
      setMessage(messageText);
      setErrorInfo(typeof error === 'object' ? JSON.stringify(error, Object.getOwnPropertyNames(error), 2) : '');
    } finally {
      setIsLoading(false);
    }
  }

  async function handleSuggest() {
    setIsLoading(true);
    try {
      const preferredDate = new Date().toISOString();
      const suggestionResponse = await getSuggestions(preferredDate);
      setSuggestion(suggestionResponse);
      setErrorInfo('');
    } catch (error) {
      const messageText = error?.message || 'Failed to load suggestions.';
      setMessage(messageText);
      setErrorInfo(typeof error === 'object' ? JSON.stringify(error, Object.getOwnPropertyNames(error), 2) : '');
    } finally {
      setIsLoading(false);
    }
  }

  async function handleAdminLogin() {
    setIsLoading(true);
    try {
      const result = await login(authForm.username, authForm.password);
      localStorage.setItem('token', result.token);
      localStorage.setItem('adminUsername', authForm.username);
      setToken(result.token);
      setAdminUsername(authForm.username);
      setMessage('Admin logged in successfully.');
      setErrorInfo('');
    } catch (error) {
      const messageText = error?.message || 'Login failed.';
      setMessage(messageText);
      setErrorInfo(typeof error === 'object' ? JSON.stringify(error, Object.getOwnPropertyNames(error), 2) : '');
    } finally {
      setIsLoading(false);
    }
  }

  function handleLogout() {
    localStorage.removeItem('token');
    localStorage.removeItem('adminUsername');
    setToken('');
    setAdminUsername('');
    setAuthForm({ username: 'admin', password: 'password' });
    setMessage('Logged out successfully.');
  }

  async function handleCreateSlot() {
    if (!token) {
      setMessage('Please login as admin before creating a slot.');
      return;
    }

    setIsLoading(true);
    try {
      await createSlot({ ...adminForm, start: new Date(adminForm.start).toISOString(), end: new Date(adminForm.end).toISOString(), capacity: Number(adminForm.capacity) }, token);
      setMessage('Appointment slot created.');
      setErrorInfo('');
      setAdminForm({ title: '', description: '', start: '', end: '', capacity: 1 });
      await loadData();
    } catch (error) {
      const messageText = error?.message || 'Failed to create slot.';
      setMessage(messageText);
      setErrorInfo(typeof error === 'object' ? JSON.stringify(error, Object.getOwnPropertyNames(error), 2) : '');
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div className="app-shell">
      <header className="app-header">
        <div className="header-content">
          <div className="header-left">
            <h1>Appointment Scheduler</h1>
            <p></p>
          </div>
          {isAdminAuthenticated && (
            <div className="header-right">
              <span className="admin-username">👤 {adminUsername}</span>
              <button className="logout-button" onClick={handleLogout}>Logout</button>
            </div>
          )}
        </div>
      </header>

      <nav className="app-nav">
        <button 
          className={`nav-button ${activeMenu === 'booking' ? 'active' : ''}`}
          onClick={() => setActiveMenu('booking')}
        >
          📅 Booking Appointment
        </button>
        <button 
          className={`nav-button ${activeMenu === 'admin' ? 'active' : ''}`}
          onClick={() => setActiveMenu('admin')}
        >
          🔐 Create Appointment Slot
        </button>
      </nav>

      <main>
        {isLoading && <div className="loading-overlay">Loading...</div>}
        {activeMenu === 'booking' && (
          <div className="content-grid booking-view">
            <div className="panel">
              <Calendar slots={slots} bookings={bookings} />
            </div>
            <div className="panel">
              <BookingForm
                slots={slots}
                onSubmit={handleBook}
                onSuggest={handleSuggest}
                suggested={suggestion}
                selectedSlotId={selectedSlotId}
                setSelectedSlotId={setSelectedSlotId}
                formValues={formValues}
                setFormValues={setFormValues}
                isLoading={isLoading}
              />
            </div>
          </div>
        )}

        {activeMenu === 'admin' && (
          <div className="content-grid admin-view">
            {!isAdminAuthenticated ? (
              <div className="panel admin-panel">
                <h2>Admin Login Required</h2>
                <section className="auth-section">
                  <h3></h3>
                  <label>
                    Username
                    <input value={authForm.username} onChange={e => setAuthForm({ ...authForm, username: e.target.value })} />
                  </label>
                  <label>
                    Password
                    <input type="password" value={authForm.password} onChange={e => setAuthForm({ ...authForm, password: e.target.value })} />
                  </label>
                  <button onClick={handleAdminLogin} disabled={isLoading}>{isLoading ? 'Logging in...' : 'Login as admin'}</button>                  
                </section>
              </div>
            ) : (
              <div className="panel admin-panel">
                <h2>Create Appointment Slot</h2>
                <section className="create-slot-section">
                  <label>
                    Title
                    <input value={adminForm.title} onChange={e => setAdminForm({ ...adminForm, title: e.target.value })} />
                  </label>
                  <label>
                    Description
                    <textarea value={adminForm.description} onChange={e => setAdminForm({ ...adminForm, description: e.target.value })} />
                  </label>
                  <label>
                    Start
                    <input type="datetime-local" value={adminForm.start} onChange={e => setAdminForm({ ...adminForm, start: e.target.value })} />
                  </label>
                  <label>
                    End
                    <input type="datetime-local" value={adminForm.end} onChange={e => setAdminForm({ ...adminForm, end: e.target.value })} />
                  </label>
                  <label>
                    Capacity
                    <input type="number" value={adminForm.capacity} min="1" onChange={e => setAdminForm({ ...adminForm, capacity: Number(e.target.value) })} />
                  </label>
                  <button onClick={handleCreateSlot} disabled={isLoading || !isAdminAuthenticated}>{isLoading ? 'Creating...' : 'Create slot'}</button>
                </section>
              </div>
            )}
          </div>
        )}

        {message && (
          <div className="toast">
            <div>{message}</div>
            {errorInfo && <pre className="toast-details">{errorInfo}</pre>}
          </div>
        )}
      </main>
    </div>
  );
}

export default App;
