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
- **Authentication**: JWT Bearer tokens with BCrypt password hashing
- **Tools**: Entity Framework Core, Swagger for API documentation, BCrypt.Net-Next for password hashing

---

## Getting Started
1. Clone the repository:
```
git clone https://github.com/Odavid66/E_Commerce_API.git
```

2. Navigate to the project directory:
```
cd E_Commerce_API
```

3. **Create `appsettings.json` with your configuration:**
   - Copy `appsettings.json.example` to `appsettings.json`
   - Update database connection string, JWT secret key, and other settings
   - ⚠️ **Never commit `appsettings.json` to Git** - it contains secrets!

4. Restore dependencies:
```
dotnet restore
```

5. Build the project:
```
dotnet build
```

6. Run migrations with EF Core:
```
dotnet ef database update
```

6. Run the application:
```
dotnet run
```

The API will be available at `https://localhost:5001` with Swagger UI at `https://localhost:5001/swagger`

---

## Authentication & API Endpoints

### Auth Endpoints
The API includes JWT-based authentication:

- **POST** `/api/auth/register` - Create a new account
  - Body: `{ email, password, firstName, lastName }`
  - Returns: 201 Created with user details

- **POST** `/api/auth/login` - Login and receive JWT token
  - Body: `{ email, password }`
  - Returns: 200 OK with JWT token and user details

- **GET** `/api/auth/profile` - Get logged-in user profile (requires JWT token)
  - Header: `Authorization: Bearer <token>`
  - Returns: 200 OK with user details

### Using JWT Tokens
1. Call `/api/auth/register` or `/api/auth/login` to get a token
2. Include the token in subsequent requests: `Authorization: Bearer <your_token_here>`
3. Tokens expire after 1 hour

### JWT Configuration
Update `appsettings.json` to change JWT settings:
```json
"Jwt": {
  "SecretKey": "your-super-secret-key-change-this-in-production",
  "Issuer": "ECommerceAPI",
  "Audience": "ECommerceUsers"
}
```

⚠️ **Important**: Change the SecretKey before deploying to production!

### Known Issues
- Using EF Core 10.0.0 (10.0.7+ had compatibility issues)
- Fixed decimal precision for currency fields

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
