# ChallengeCodere - README

## Introduction
ChallengeCodere is a .NET Core Web API application that integrates with the TVMaze API to fetch and store show data. The application allows you to manage TV shows, including details about their networks, genres, ratings, and more. It provides a robust, layered architecture with clean code principles and follows best practices in API development.

## Technologies Used
- **.NET 6** - Framework for building the Web API.
- **Entity Framework Core** - ORM for managing database operations.
- **In-Memory Database** - Used in unit tests for testing database operations.
- **SQL Server** - Relational database for data persistence.
- **Swagger** - Documentation and testing interface for the API.
- **Moq** - Mocking framework for unit tests.
- **xUnit** - Test framework for unit testing.
- **FluentAssertions** - Library for expressive test assertions.

## Prerequisites
- **.NET SDK 6** or higher.
- **SQL Server** - Ensure a local or remote instance is running.
- **Visual Studio 2022** or later (recommended) or any other IDE that supports .NET development.
- **Postman** or any other tool for testing the API (optional).

## Project Setup
1. **Clone the repository**:
   ```bash
   git clone https://github.com/ezequielmm/ChallengeCodere.git
   cd ChallengeCodere
   ```

2. **Restore NuGet packages**:
   ```bash
   dotnet restore
   ```

3. **Update `appsettings.json`**:
   Ensure the connection string and other configurations are correctly set up (refer to Database Configuration).

4. **Run database migrations**:
   ```bash
   dotnet ef database update
   ```

5. **Build and run the project**:
   ```bash
   dotnet run
   ```

6. **Access the API documentation**:
   Navigate to `https://localhost:44330/swagger/index.html` to interact with the API using Swagger UI.

## Database Configuration
1. **Update `appsettings.json`**:
   Configure the connection string for SQL Server in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TvMazeShowsDb;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```
2. **Run the migrations** to create the necessary tables in the database:
   ```bash
   dotnet ef database update
   ```

3. **Verify the database**:
   Ensure that the `Shows`, `Networks`, `Genres`, `Ratings`, `Externals`, and `Countries` tables are created in the SQL Server database.

## API Key Configuration
The application interacts with the external TVMaze API. If an API key is required for your API provider:
1. **Store the API key** in `appsettings.json` or in an environment variable:
   ```json
   "TvMazeApi": {
     "BaseUrl": "http://api.tvmaze.com",
     "ApiKey": "67890-klmno-12345-pqrst"
   }
   ```
2. **Access the API key** in the application:
   The `IHttpClientFactory` setup in the project will use this configuration to attach the API key to each request.

## Running the Project with IIS
1. **Open the project in Visual Studio**.
2. **Right-click the project** in Solution Explorer and select **Properties**.
3. **Go to the Debug tab** and set **Profile** to **IIS**.
4. **Configure IIS**:
   - Ensure that **IIS** is installed with **ASP.NET Core Hosting Bundle**.
   - Create a new site in IIS pointing to the `ChallengeCodere` project's published folder.
   - Update the **Application Pool** to use **No Managed Code**.
5. **Publish the project**:
   ```bash
   dotnet publish -c Release -o ./publish
   ```
6. **Access the API**:
   The API should be accessible at the configured IIS site URL

## Project Structure
- **Controllers**
  - `ShowsController.cs`: Handles the HTTP requests for managing TV shows.
- **Services**
  - `ShowService.cs`: Contains business logic for interacting with shows.
- **Repositories**
  - `ShowRepository.cs`: Manages CRUD operations for shows using Entity Framework.
- **DTOs**
  - `ShowResponseDto.cs`, `NetworkResponseDto.cs`, etc.: Data Transfer Objects to standardize the data returned by the API.
- **Models**
  - `Show.cs`, `Network.cs`, `Genre.cs`: Entity models that represent the database schema.
- **Data**
  - `ApplicationDbContext.cs`: The Entity Framework Core context for interacting with the database.
- **Tests**
  - `ShowServiceTests.cs`, `ShowRepositoryTests.cs`, `ShowsControllerTests.cs`: Unit tests for different layers of the application using xUnit and Moq.

## Design Patterns
- **Repository Pattern**: Abstracts data access logic and separates it from business logic. This makes the `ShowRepository` responsible for managing database interactions, while the `ShowService` handles business rules.
- **Service Layer Pattern**: Contains business logic and interacts with repositories. This keeps controllers lean and focused on handling HTTP requests.
- **DTO Pattern (Data Transfer Object)**: Used to transfer data between the service and the client, ensuring only the necessary data is exposed, avoiding over-posting or under-posting issues.
- **Dependency Injection**: Used throughout the application to inject services and repositories, making the code more testable and maintainable.
- **Unit of Work**: Managed implicitly through the use of the `DbContext`, allowing grouped database transactions.

## Unit Testing Approach
- **Testing Framework**: Uses `xUnit` for testing due to its simplicity and support for parallel test execution.
- **Mocking Dependencies**: Uses `Moq` to mock dependencies like repositories and `IHttpClientFactory`, allowing isolated tests for the service and controller layers.
- **In-Memory Database**: Uses `Microsoft.EntityFrameworkCore.InMemory` for repository tests, providing a lightweight, in-memory database for testing database interactions without affecting the production database.
- **Test Structure**:
  - **Repository Tests**: Verify that the `ShowRepository` correctly interacts with the in-memory database.
  - **Service Tests**: Ensure that `ShowService` processes business logic correctly and calls repository methods as expected.
  - **Controller Tests**: Check the HTTP responses (e.g., `200 OK`, `404 NotFound`) for different API endpoints and validate the data returned.