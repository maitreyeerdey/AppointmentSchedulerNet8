using AppointmentScheduler.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpClient("AppointmentService", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["Services:AppointmentService"]!);
});

builder.Services.AddHttpClient("BookingService", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["Services:BookingService"]!);
});


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

/*

builder.Services.AddHttpClient("AppointmentService", client =>
{
    client.BaseAddress =
        new Uri("http://localhost:5089/");
        //for Production
    client.BaseAddress =
        new Uri("https://appointmentservice.yellowpebble-e1ad0743.centralindia.azurecontainerapps.io/");
});


builder.Services.AddHttpClient("BookingService", client =>
{
    client.BaseAddress =
        new Uri("http://localhost:5097/");
        //for Production
    client.BaseAddress =
        new Uri("https://bookingservice.yellowpebble-e1ad0743.centralindia.azurecontainerapps.io/");
});
*/

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT key is required.");
var issuer = jwtSettings["Issuer"] ?? "AppointmentScheduler";
var audience = jwtSettings["Audience"] ?? "AppointmentClients";
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = signingKey
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
