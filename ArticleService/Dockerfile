FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Common.Shared/Common.Shared.csproj", "Common.Shared/"]
COPY ["ArticleService/ArticleService.csproj", "ArticleService/"]
RUN dotnet restore "ArticleService/ArticleService.csproj"

COPY . .
WORKDIR /src/ArticleService
RUN dotnet publish "ArticleService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ArticleService.dll"]
