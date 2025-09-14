Brewery API

Enterprise-grade RESTful API for brewery information with advanced search, filtering, and sorting capabilities.

Features

Core Features
- RESTful API - Clean, predictable endpoints following REST conventions
- Search & Filter - Advanced search by name, city, state with partial matching
- Sorting - Sort by name, city, or distance (with user location)
- Pagination - Efficient handling of large datasets
- Caching - 10-minute intelligent caching with automatic refresh
- Error Handling - Comprehensive error handling with structured responses

Advanced Features
- Autocomplete - Intelligent search suggestions
- Distance Calculation - Haversine formula for geographical proximity
- API Versioning - Multiple versioning strategies (URL, header, query string)
- Logging - Structured logging throughout the application
- Clean Architecture - SOLID principles with dependency injection
- Security Ready - CORS, authentication-ready structure

Getting Started

Prerequisites
- .NET 8.0 SDK or later
- Visual Studio 2022 / VS Code / JetBrains Rider (optional)

Installation

1. Clone or extract the project:
   cd BreweryApi

2. Restore dependencies:
   dotnet restore

3. Build the solution:
   dotnet build

4. Run the application:
   dotnet run --project Brewery.Api

API Documentation
Swagger UI is available at:
https://localhost:<port>/swagger

Project Structure
- Brewery.Api - API layer (controllers, middleware, versioning, etc.)
- Brewery.Application - Business logic and service layer
- Brewery.Domain - Entities, models, and core abstractions
- Brewery.Infrastructure - Persistence, repositories, caching, logging

License
This project is licensed under the MIT License.
