FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Common.Shared/Common.Shared.csproj", "Common.Shared/"]
COPY ["DraftService/DraftService.csproj", "DraftService/"]
RUN dotnet restore "DraftService/DraftService.csproj"

COPY . .
WORKDIR /src/DraftService
RUN dotnet publish "DraftService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "DraftService.dll"]
