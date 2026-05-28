FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AppointmentScheduler.AppointmentService/AppointmentScheduler.AppointmentService.csproj", "AppointmentScheduler.AppointmentService/"]
COPY ["AppointmentScheduler.Shared/AppointmentScheduler.Shared.csproj", "AppointmentScheduler.Shared/"]
RUN dotnet restore "AppointmentScheduler.AppointmentService/AppointmentScheduler.AppointmentService.csproj"
COPY . .
WORKDIR "/src/AppointmentScheduler.AppointmentService"
RUN dotnet build "AppointmentScheduler.AppointmentService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AppointmentScheduler.AppointmentService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppointmentScheduler.AppointmentService.dll"]
