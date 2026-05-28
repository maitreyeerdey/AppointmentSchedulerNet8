using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace AppointmentScheduler.Gateway.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentProxyController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AppointmentProxyController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    [HttpGet("{*route}")]

    [HttpPost]
    [HttpPost("{*route}")]

    [HttpPut]
    [HttpPut("{*route}")]

    [HttpDelete]
    [HttpDelete("{*route}")]

    [HttpPatch]
    [HttpPatch("{*route}")]
    public async Task<IActionResult> Proxy(
        [FromRoute] string? route,
        [FromQuery(Name = "route")] string? routeQuery)
    {
        var actualRoute = route ?? routeQuery;

        if (string.IsNullOrWhiteSpace(actualRoute))
        {
            return BadRequest(new
            {
                message =
                    "Route is required. Example: api/v1/slots"
            });
        }

        return await ForwardRequestAsync(
            "AppointmentService",
            actualRoute);
    }

    private async Task<IActionResult> ForwardRequestAsync(
        string clientName,
        string route)
    {
        var client =
            _httpClientFactory.CreateClient(clientName);

        var downstreamUri =
            BuildDownstreamUri(route);

        var request = new HttpRequestMessage(
            new HttpMethod(Request.Method),
            downstreamUri);

        // Copy body
        if (!HttpMethods.IsGet(Request.Method) &&
            !HttpMethods.IsHead(Request.Method))
        {
            request.Content =
                new StreamContent(Request.Body);

            if (!string.IsNullOrWhiteSpace(Request.ContentType) &&
                MediaTypeHeaderValue.TryParse(
                    Request.ContentType,
                    out var mediaType))
            {
                request.Content.Headers.ContentType =
                    mediaType;
            }
        }

        // Copy headers
        foreach (var header in Request.Headers)
        {
            if (header.Key.Equals(
                    "Host",
                    StringComparison.OrdinalIgnoreCase) ||

                header.Key.Equals(
                    "Content-Type",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            request.Headers.TryAddWithoutValidation(
                header.Key,
                header.Value.ToArray());
        }

        var response = await client.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead);

        var content =
            await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            StatusCode = (int)response.StatusCode,
            Content = content,
            ContentType =
                response.Content.Headers.ContentType?.ToString()
                ?? "application/json"
        };
    }

    private string BuildDownstreamUri(string route)
    {
        var normalizedRoute =
            NormalizeRoute(route);

        var queryParameters =
            Request.Query
                .Where(q =>
                    !string.Equals(
                        q.Key,
                        "route",
                        StringComparison.OrdinalIgnoreCase))
                .SelectMany(
                    q => q.Value,
                    (q, value) =>
                        new KeyValuePair<string, string>(
                            q.Key,
                            value!))
                .ToList();

        if (!queryParameters.Any())
        {
            return normalizedRoute;
        }

        var queryString = string.Join(
            "&",
            queryParameters.Select(p =>
                $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));

        return $"{normalizedRoute}?{queryString}";
    }

    private static string NormalizeRoute(string route)
    {
        var normalized =
            route.TrimStart('/');

        if (!normalized.StartsWith(
                "api/",
                StringComparison.OrdinalIgnoreCase))
        {
            normalized = $"api/{normalized}";
        }

        return normalized;
    }
}

[ApiController]
[Route("api/bookings")]
public class BookingProxyController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BookingProxyController(
        IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    [HttpGet("{*route}")]

    [HttpPost]
    [HttpPost("{*route}")]

    [HttpPut]
    [HttpPut("{*route}")]

    [HttpDelete]
    [HttpDelete("{*route}")]

    [HttpPatch]
    [HttpPatch("{*route}")]
    public async Task<IActionResult> Proxy(
        [FromRoute] string? route,
        [FromQuery(Name = "route")] string? routeQuery)
    {
        var actualRoute = route ?? routeQuery;

        if (string.IsNullOrWhiteSpace(actualRoute))
        {
            return BadRequest(new
            {
                message =
                    "Route is required. Example: api/v1/bookings"
            });
        }

        return await ForwardRequestAsync(
            "BookingService",
            actualRoute);
    }

    private async Task<IActionResult> ForwardRequestAsync(
        string clientName,
        string route)
    {
        var client =
            _httpClientFactory.CreateClient(clientName);

        var downstreamUri =
            BuildDownstreamUri(route);

        var request = new HttpRequestMessage(
            new HttpMethod(Request.Method),
            downstreamUri);

        if (!HttpMethods.IsGet(Request.Method) &&
            !HttpMethods.IsHead(Request.Method))
        {
            request.Content =
                new StreamContent(Request.Body);

            if (!string.IsNullOrWhiteSpace(Request.ContentType) &&
                MediaTypeHeaderValue.TryParse(
                    Request.ContentType,
                    out var mediaType))
            {
                request.Content.Headers.ContentType =
                    mediaType;
            }
        }

        foreach (var header in Request.Headers)
        {
            if (header.Key.Equals(
                    "Host",
                    StringComparison.OrdinalIgnoreCase) ||

                header.Key.Equals(
                    "Content-Type",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            request.Headers.TryAddWithoutValidation(
                header.Key,
                header.Value.ToArray());
        }

        var response = await client.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead);

        var content =
            await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            StatusCode = (int)response.StatusCode,
            Content = content,
            ContentType =
                response.Content.Headers.ContentType?.ToString()
                ?? "application/json"
        };
    }

    private string BuildDownstreamUri(string route)
    {
        var normalizedRoute =
            NormalizeRoute(route);

        var queryParameters =
            Request.Query
                .Where(q =>
                    !string.Equals(
                        q.Key,
                        "route",
                        StringComparison.OrdinalIgnoreCase))
                .SelectMany(
                    q => q.Value,
                    (q, value) =>
                        new KeyValuePair<string, string>(
                            q.Key,
                            value!))
                .ToList();

        if (!queryParameters.Any())
        {
            return normalizedRoute;
        }

        var queryString = string.Join(
            "&",
            queryParameters.Select(p =>
                $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));

        return $"{normalizedRoute}?{queryString}";
    }

    private static string NormalizeRoute(string route)
    {
        var normalized =
            route.TrimStart('/');

        if (!normalized.StartsWith(
                "api/",
                StringComparison.OrdinalIgnoreCase))
        {
            normalized = $"api/{normalized}";
        }

        return normalized;
    }
}