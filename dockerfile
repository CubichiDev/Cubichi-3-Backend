FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY ["./out", "./"]

CMD ["dotnet", "Cubichi-Backend"] 