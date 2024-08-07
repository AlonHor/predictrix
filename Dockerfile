FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Predictrix/Predictrix.csproj", "Predictrix/"]
COPY ["Predictrix.Application/Predictrix.Application.csproj", "Predictrix.Application/"]
COPY ["Predictrix.Domain/Predictrix.Domain.csproj", "Predictrix.Domain/"]
COPY ["Predictrix.Infrastructure/Predictrix.Infrastructure.csproj", "Predictrix.Infrastructure/"]
COPY ["Serilog.Sinks.Discord/Serilog.Sinks.Discord.csproj", "Serilog.Sinks.Discord/"]
RUN dotnet restore "Predictrix/Predictrix.csproj"
COPY . .
WORKDIR "/src/Predictrix"
RUN dotnet build "Predictrix.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Predictrix.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS http://+:$PORT
CMD dotnet Predictrix.dll
