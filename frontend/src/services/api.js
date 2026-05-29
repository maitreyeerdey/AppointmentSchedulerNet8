//const apiBase = '/api';
const apiBase =
  'https://appschedulergateway.yellowpebble-e1ad0743.centralindia.azurecontainerapps.io/api';
  //'https://appschedulergateway.yellowpebble-e1ad0743.centralindia.azurecontainerapps.io/api';

async function request(path, options = {}) {
  const { headers: optionHeaders, ...restOptions } = options;
  const headers = {
    Accept: 'application/json',
    ...(optionHeaders || {})
  };

  if (options.body) {
    headers['Content-Type'] = headers['Content-Type'] || 'application/json';
  }

  const response = await fetch(`${apiBase}${path}`, {
    headers,
    credentials: 'same-origin',
    ...restOptions
  });

  const text = await response.text();
  let data = null;
  const contentType = response.headers.get('Content-Type') || '';

  if (text) {
    if (contentType.includes('application/json')) {
      try {
        data = JSON.parse(text);
      } catch {
        data = { message: text };
      }
    } else {
      data = { message: text };
    }
  }

  if (!response.ok) {
    throw new Error(data?.message || response.statusText || 'Request failed');
  }

  return data;
}

export function login(username, password) {
  return request('/v1/auth/login', {
    method: 'POST',
    body: JSON.stringify({ username, password })
  });
}

export function getSlots() {
  return request('/appointments/v1/slots');
}

export function getAvailableSlots() {
  return request('/appointments/v1/slots/available');
}

export function getSuggestions(preferredDate) {
  return request(`/appointments/v1/suggestions?preferredDate=${encodeURIComponent(preferredDate)}`);
}

export function createSlot(slot, token) {
  return request('/appointments/v1/slots', {
    method: 'POST',
    headers: { Authorization: `Bearer ${token}` },
    body: JSON.stringify(slot)
  });
}

export function createBooking(booking) {
  return request('/bookings/v1/bookings', {
    method: 'POST',
    body: JSON.stringify(booking)
  });
}

export function getBookings() {
  return request('/bookings/v1/bookings');
}
