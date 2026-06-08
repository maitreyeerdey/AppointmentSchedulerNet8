import { describe, it, expect, vi, beforeEach } from 'vitest';
import * as api from '../src/services/api';

beforeEach(() => {
  global.fetch = vi.fn();
});

describe('api client', () => {
  it('parses JSON responses and returns data', async () => {
    const body = JSON.stringify({ hello: 'world' });
    global.fetch.mockResolvedValue({
      ok: true,
      text: async () => body,
      headers: { get: () => 'application/json' }
    });

    const data = await api.getSlots();
    expect(data).toEqual({ hello: 'world' });
    expect(global.fetch).toHaveBeenCalled();
  });

  it('throws an Error when response is not ok and contains message', async () => {
    const body = JSON.stringify({ message: 'Bad things' });
    global.fetch.mockResolvedValue({
      ok: false,
      text: async () => body,
      headers: { get: () => 'application/json' },
      statusText: 'Bad'
    });

    await expect(api.getSlots()).rejects.toThrow('Bad things');
  });

  it('handles plain text responses', async () => {
    const body = 'plain text error';
    global.fetch.mockResolvedValue({
      ok: false,
      text: async () => body,
      headers: { get: () => 'text/plain' },
      statusText: 'Bad'
    });

    await expect(api.getSlots()).rejects.toThrow('plain text error');
  });

  it('createBooking sends correct path and body', async () => {
    global.fetch.mockResolvedValue({
      ok: true,
      text: async () => JSON.stringify({ id: 1 }),
      headers: { get: () => 'application/json' }
    });

    const booking = { appointmentSlotId: 1, customerName: 'A', customerEmail: 'a@b.com', notes: '' };
    const res = await api.createBooking(booking);

    expect(res).toEqual({ id: 1 });
    expect(global.fetch).toHaveBeenCalledWith(expect.stringContaining('/api/bookings/v1/bookings'), expect.objectContaining({ method: 'POST' }));
  });
});
