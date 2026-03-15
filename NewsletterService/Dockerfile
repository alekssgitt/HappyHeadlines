FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Common.Shared/Common.Shared.csproj", "Common.Shared/"]
COPY ["NewsletterService/NewsletterService.csproj", "NewsletterService/"]
RUN dotnet restore "NewsletterService/NewsletterService.csproj"

COPY . .
WORKDIR /src/NewsletterService
RUN dotnet publish "NewsletterService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "NewsletterService.dll"]
