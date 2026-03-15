FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Common.Shared/Common.Shared.csproj", "Common.Shared/"]
COPY ["PublisherService/PublisherService.csproj", "PublisherService/"]
RUN dotnet restore "PublisherService/PublisherService.csproj"

COPY . .
WORKDIR /src/PublisherService
RUN dotnet publish "PublisherService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "PublisherService.dll"]
