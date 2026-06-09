FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Common.Shared/Common.Shared.csproj", "Common.Shared/"]
COPY ["ProfanityService/ProfanityService.csproj", "ProfanityService/"]
RUN dotnet restore "ProfanityService/ProfanityService.csproj"

COPY . .
WORKDIR /src/ProfanityService
RUN dotnet publish "ProfanityService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ProfanityService.dll"]
