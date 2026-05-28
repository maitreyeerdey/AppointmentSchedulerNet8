FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AppointmentScheduler.Gateway/AppointmentScheduler.Gateway.csproj", "AppointmentScheduler.Gateway/"]
COPY ["AppointmentScheduler.Shared/AppointmentScheduler.Shared.csproj", "AppointmentScheduler.Shared/"]
RUN dotnet restore "AppointmentScheduler.Gateway/AppointmentScheduler.Gateway.csproj"
COPY . .
WORKDIR "/src/AppointmentScheduler.Gateway"
RUN dotnet build "AppointmentScheduler.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AppointmentScheduler.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppointmentScheduler.Gateway.dll"]
