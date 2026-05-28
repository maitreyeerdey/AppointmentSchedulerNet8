FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AppointmentScheduler.BookingService/AppointmentScheduler.BookingService.csproj", "AppointmentScheduler.BookingService/"]
COPY ["AppointmentScheduler.Shared/AppointmentScheduler.Shared.csproj", "AppointmentScheduler.Shared/"]
RUN dotnet restore "AppointmentScheduler.BookingService/AppointmentScheduler.BookingService.csproj"
COPY . .
WORKDIR "/src/AppointmentScheduler.BookingService"
RUN dotnet build "AppointmentScheduler.BookingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AppointmentScheduler.BookingService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppointmentScheduler.BookingService.dll"]
