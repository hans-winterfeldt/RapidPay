# RapidPay Application

This project is a simple payment sistem that allows users to create cards, check their balances, and process payments. It includes a dynamic fee calculation system and secure authentication using JWT. The application is built using ASP.NET Core and follows a modular architecture for easy maintenance and scalability.

---

## Features

### Card Management Module
- **Create Card**: Create a new card with a 15-digit card number and an initial balance.
- **Get Card Balance**: Retrieve the current balance of a card.
- **Process Payment**: Deduct a payment amount (including fees) from the card's balance.

### Payment Fees Module
- **Dynamic Fee Calculation**: The Universal Fees Exchange (UFE) generates a random fee multiplier every hour, which is applied to payments.

### Bonus Features
- **Secure Authentication**: We use JWT for secure and stateless authentication.
- **Multithreading**: Improved API throughput with asynchronous programming.

---

## Project Structure
We followed a clean architecture approach to separate concerns and improve maintainability. The project is divided into the following layers:

- **API**: Contains the ASP.NET Core Web API controllers and configuration.
- **Application**: Contains the business logic and service interfaces.
- **Domain**: Contains the core domain entities and value objects.
- **Infrastructure**: Contains the data access layer and external service integrations.
- **Tests**: Contains unit and integration tests for the application.

## Design Patterns and Best Practices
- **Singleton Pattern**: Used for the UFE service to ensure a single instance is used throughout the application.
- **JWT Authentication**: Securely authenticates users and protects API endpoints.
- **Dependency Injection**: ASP.NET Core's built-in DI is used to manage service lifetimes and dependencies.
- **CQRS**: Command Query Responsibility Segregation is used to separate read and write operations for better performance and scalability.
- **Entity Framework Core**: Used for data access and ORM capabilities.
- **AAA (Arrange, Act, Assert)**: Used in unit tests to ensure clear and maintainable test cases, using FakeItEasy, Bogus and Shouldly Packages to implement the tests.
