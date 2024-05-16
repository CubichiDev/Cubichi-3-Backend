# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080 5432

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Cubichi-Backend.sln", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build "Cubichi-Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cubichi-Backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Controllers.dll"]