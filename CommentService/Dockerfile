FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Common.Shared/Common.Shared.csproj", "Common.Shared/"]
COPY ["CommentService/CommentService.csproj", "CommentService/"]
RUN dotnet restore "CommentService/CommentService.csproj"

COPY . .
WORKDIR /src/CommentService
RUN dotnet publish "CommentService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "CommentService.dll"]
