<<<<<<< HEAD
<<<<<<< HEAD
# Appointment Scheduler

This repository contains a full-stack appointment scheduling application built with:

- ASP.NET Core Web API (.NET 8)
- SQL Server / LocalDB with Entity Framework Core
- React.js frontend using Vite
- API Gateway with custom proxy
- JWT authentication, health checks, logging, validation, API versioning
- A microservices-style architecture with Appointment and Booking services

## Projects

- `AppointmentScheduler.AppointmentService` - manages appointment slots
- `AppointmentScheduler.BookingService` - creates bookings and coordinates with the appointment service
- `AppointmentScheduler.Gateway` - reverse proxy and authentication gateway
- `AppointmentScheduler.Shared` - shared DTO models
- `AppointmentScheduler.Tests` - xUnit tests
- `frontend` - React UI

## Run locally

1. Open a terminal in `d:\AppintmentScheduler`
2. Run migrations or let EF create the database at runtime
3. Start the services:
   - `dotnet run --project AppointmentScheduler.AppointmentService\AppointmentScheduler.AppointmentService.csproj`
   - `dotnet run --project AppointmentScheduler.BookingService\AppointmentScheduler.BookingService.csproj`
   - `dotnet run --project AppointmentScheduler.Gateway\AppointmentScheduler.Gateway.csproj`
4. Start the frontend:
   - `cd frontend`
   - `npm run dev`

## Run with Docker Compose

1. Ensure Docker and Docker Compose are installed.
2. Run `docker-compose up --build` from the root directory.
3. Access the app:
   - Frontend: http://localhost:5173
   - Gateway API: http://localhost:5068
   - Appointment Service: http://localhost:5089
   - Booking Service: http://localhost:5097

## Authentication

Use the admin credentials to create appointment slots:

- Username: `admin`
- Password: `password`

## Features

- Create appointment slots
- Book appointments through the booking service
- View schedule and calendar list
- API gateway with reverse proxy routing
- JWT login endpoint
- Basic AI-style suggestion endpoint
- Health checks on all backend services
- Clean architecture and dependency injection

## Notes

- Update connection strings in `appsettings.json` if you want to point at a real SQL Server instance.
- The frontend proxies `/api` to the gateway at `http://localhost:5068`.
- Add more services, event buses, and identity providers as the architecture grows.
=======
# appointmentscheduler
>>>>>>> 1fb9533779727a0d0c358f14f5312f603831c394
=======
# appointmentscheduler
>>>>>>> 1fb9533779727a0d0c358f14f5312f603831c394
