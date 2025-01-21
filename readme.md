# Cubichi Backend

Cubichi Backend is a .NET 8.0 web API project that provides authentication and skin management services. The project uses JWT for authentication and PostgreSQL for data storage.

## API Endpoints

### Authentication

- **Register**
  - **URL:** `/api/auth/register`
  - **Method:** `POST`
  - **Request Body:**

    ```json
    {
      "UserName": "string",
      "Email": "string",
      "Password": "string",
      "ConfirmPassword": "string"
    }
    ```

  - **Response:**

    ```json
    {
      "Token": "string"
    }
    ```

- **Login**
  - **URL:** `/api/auth/login`
  - **Method:** `POST`
  - **Request Body:**

    ```json
    {
      "UserName": "string",
      "Password": "string"
    }
    ```

  - **Response:**

    ```json
    {
      "Token": "string"
    }
    ```

### Skin Management

- **Get Skin**
  - **URL:** `/api/skin/{userName}`
  - **Method:** `GET`
  - **Response:** Returns the skin image file.

- **Upload Skin**
  - **URL:** `/api/skin/upload`
  - **Method:** `POST`
  - **Authorization:** Bearer Token
  - **Request Body:** `multipart/form-data` with a file field.
  - **Response:** `200 OK` on success.

## Environment Variables

The project uses the following environment variables:

- **JWT_SECRET:** Secret key used for signing JWT tokens.
- **DB_HOST:** Hostname of the PostgreSQL database.
- **DB_PORT:** Port number of the PostgreSQL database.
- **DB_NAME:** Name of the PostgreSQL database.
- **DB_USER:** Username for the PostgreSQL database.
- **DB_PASSWORD:** Password for the PostgreSQL database.

## Docker

The project includes a Dockerfile and a docker-compose.yaml file for containerization.

### Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY ["./out", "./"]

CMD ["dotnet", "Cubichi-Backend"]
```

### Docker Compose

```yaml
version: '3.1'

services:
  endpoint:
    restart: always
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - 8083:8080
    environment:
      JWT_SECRET: ${JWT_SECRET}
```

## License

This project is licensed under the MIT License.
