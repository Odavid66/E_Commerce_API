# E-Commerce API

This repository contains the source code for an E-Commerce Web API built with .NET 10 and C#. The project is a collaborative effort to reinforce knowledge of building RESTful APIs.

---

## Table of Contents
- [Project Overview](#project-overview)
- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
- [Branch Naming Conventions](#branch-naming-conventions)
- [Contributing](#contributing)
- [License](#license)

---

## Project Overview
The E-Commerce API provides backend functionality for an e-commerce platform, including:
- User management
- Product catalog
- Payment processing
- Order management

This project is designed to follow clean architecture principles and best practices for .NET development.

---

## Technologies Used
- **Framework**: .NET 10    
- **Language**: C#
- **Database**: SQL Server
- **Tools**: Entity Framework Core, Swagger for API documentation

---

## Getting Started
1. Clone the repository:
git clone https://github.com/Odavid66/E_Commerce_API_Csharp.git

2. Navigate to the project directory:
cd E_Commerce_API_Csharp

3. Restore dependencies:
dotnet restore

4. Build the project:
dotnet build

5. Run the application:
dotnet run

Note: Run migrations with EF Core

### Known Issues
- Using EF Core 10.0.0 (10.0.7+ had compatibility issues)


---

## Branch Naming Conventions
To maintain consistency, follow these branch naming conventions:
- **Feature branches**: `feature/<description>` (e.g., `feature/user-payment-entities`)
- **Bugfix branches**: `bugfix/<description>` (e.g., `bugfix/fix-login-issue`)
- **Hotfix branches**: `hotfix/<description>` (e.g., `hotfix/critical-payment-bug`)
- **Release branches**: `release/<version>` (e.g., `release/v1.0.0`)

---

## Contributing
We welcome contributions from all team members! To contribute:
1. Create a new branch following the naming conventions.
2. Make your changes and commit them with clear messages.
3. Push your branch to the remote repository.
4. Open a pull request for review.

---

## License
This project is licensed under the [MIT License](LICENSE).


---

## Authors
- Babafemi(https://github.com/Femitun)
- David(https://github.com/Odavid66)
- Emmanuel(https://github.com/voke20)
