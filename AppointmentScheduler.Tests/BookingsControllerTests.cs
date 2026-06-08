using AppointmentScheduler.BookingService.Controllers;
using AppointmentScheduler.BookingService.Entities;
using AppointmentScheduler.BookingService.Repositories;
using AppointmentScheduler.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Xunit;

namespace AppointmentScheduler.Tests;

public class BookingsControllerTests
{
    [Fact]
    public async Task Create_ReturnsBadRequest_WhenAppointmentSlotDoesNotExist()
    {
        using var client = CreateHttpClient(request =>
        {
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        var repository = new FakeBookingRepository();
        var controller = new BookingsController(repository, new FakeHttpClientFactory(client));

        var result = await controller.Create(new BookingCreateDto(1, "Jane Doe", "jane@example.com", "Notes"));

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequest.Value);
        Assert.Contains("Selected appointment slot does not exist.", badRequest.Value!.ToString() ?? string.Empty);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenBookingSucceeds()
    {
        var handler = new TestHttpMessageHandler(request =>
        {
            if (request.Method == HttpMethod.Get && request.RequestUri!.AbsolutePath.Contains("/slots/1"))
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }

            if (request.Method == HttpMethod.Post && request.RequestUri!.AbsolutePath.Contains("/slots/1/book"))
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }

            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        });

        using var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") };
        var repository = new FakeBookingRepository();
        var controller = new BookingsController(repository, new FakeHttpClientFactory(client));

        var result = await controller.Create(new BookingCreateDto(1, "Jane Doe", "jane@example.com", "Booking notes"));

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var dto = Assert.IsType<BookingDto>(created.Value!);

        Assert.Equal(1, dto.AppointmentSlotId);
        Assert.Equal("Jane Doe", dto.CustomerName);
        Assert.Equal("jane@example.com", dto.CustomerEmail);
        Assert.Equal("Booking notes", dto.Notes);
        Assert.Equal("GetBySlot", created.ActionName);
        Assert.NotEqual(default, dto.BookingDateUtc);
        Assert.Single(repository.AddedBookings);
    }

    private static HttpClient CreateHttpClient(Func<HttpRequestMessage, HttpResponseMessage> callback)
    {
        var handler = new TestHttpMessageHandler(callback);
        return new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") };
    }

    private class FakeBookingRepository : IBookingRepository
    {
        public List<Booking> AddedBookings { get; } = new();

        public Task<List<Booking>> GetAllAsync() => Task.FromResult(new List<Booking>());
        public Task<List<Booking>> GetBySlotIdAsync(int slotId) => Task.FromResult(new List<Booking>());
        public Task<Booking> AddAsync(Booking booking)
        {
            booking.Id = 1;
            AddedBookings.Add(booking);
            return Task.FromResult(booking);
        }
    }

    private class FakeHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpClient _client;

        public FakeHttpClientFactory(HttpClient client)
        {
            _client = client;
        }

        public HttpClient CreateClient(string name) => _client;
    }

    private class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _callback;

        public TestHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> callback)
        {
            _callback = callback;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_callback(request));
        }
    }
}
