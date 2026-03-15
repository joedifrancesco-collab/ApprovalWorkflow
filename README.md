# ApprovalWorkflow

A comprehensive .NET web application for managing workflow approvals, built with ASP.NET Core and following Clean Architecture principles.

## 📋 Table of Contents

- [About](#about)
- [Features](#features)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [API Endpoints](#api-endpoints)
- [Contributing](#contributing)
- [License](#license)

## About

ApprovalWorkflow is an experimental project designed to demonstrate building a modern .NET web application for managing approval requests. The system allows users to submit requests for approval cards, track their status, and manage the entire approval workflow lifecycle. This project was developed as a learning exercise to explore building .NET web applications with Claude Code.

## Features

### Core Functionality
- **Request Management** - Create, view, and track approval requests
- **User Authentication** - Secure login system with password reset functionality
- **Role-Based Access Control** - Support for different user roles (Requestor, Approver, Admin)
- **Approval Workflow** - Multi-step approval process with status tracking
- **Audit Logging** - Complete audit trail of all request changes and actions
- **Card Management** - Track card printing status and expiration dates

### Administrative Features
- **User Management** - Create, update, and manage user accounts
- **Data Management** - Manage locations, statuses, tiers, and roles
- **Lookup Management** - Configure system-wide lookup values

### Planned Features
- **Email Notifications** - Automated email notifications for approval/rejection (in backlog)

## Architecture

The application follows Clean Architecture principles with clear separation of concerns:

- **ApprovalWorkflow.API** - RESTful API layer (ASP.NET Core)
- **ApprovalWorkflow.Application** - Application business logic and DTOs
- **ApprovalWorkflow.Domain** - Domain models and entities
- **ApprovalWorkflow.Infrastructure** - Data access and external services

## Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- [SQL Server](https://www.microsoft.com/sql-server) or SQL Server Express
- [PowerShell](https://docs.microsoft.com/powershell/scripting/install/installing-powershell) (for using the start script)

## Installation

### 1. Clone the Repository

```bash
git clone https://github.com/joedifrancesco-collab/ApprovalWorkflow.git
cd ApprovalWorkflow
```

### 2. Configure the Database

Update the connection string in `ApprovalWorkflow.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ApprovalWorkflow;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Apply Database Migrations

```bash
cd ApprovalWorkflow.API
dotnet ef database update
```

### 4. Configure Email Settings (Optional)

If you plan to use email notifications, update the SMTP settings in `appsettings.json`:

```json
{
  "SmtpSettings": {
    "Host": "your-smtp-server.com",
    "Port": 587,
    "Username": "your-email@domain.com",
    "Password": "your-password",
    "FromAddress": "noreply@yourdomain.com"
  }
}
```

### 5. Build the Solution

```bash
dotnet build
```

## Usage

### Running the Application

#### Option 1: Using the PowerShell Script (Recommended)

From the solution root directory:

```powershell
.\start.ps1
```

This script will:
- Stop any existing instances
- Start the API server
- Wait for the API to be ready
- Automatically open your browser to the application

#### Option 2: Using .NET CLI

```bash
cd ApprovalWorkflow.API
dotnet run
```

Then navigate to `http://localhost:5268` in your browser.

#### Option 3: Using Visual Studio

1. Open `ApprovalWorkflow.slnx`
2. Set `ApprovalWorkflow.API` as the startup project
3. Press F5 to run

### Default Access

The application will be available at:
- **API**: `http://localhost:5268`
- **Swagger Documentation**: `http://localhost:5268/swagger`

### Creating Your First User

Use the `/api/users` endpoint to create a user account, or seed initial users through the database.

## Project Structure

```
ApprovalWorkflow/
├── ApprovalWorkflow.API/              # Web API layer
│   ├── Controllers/                   # API controllers
│   │   ├── ApprovalRequestsController.cs
│   │   ├── AuthController.cs
│   │   ├── DataManagersController.cs
│   │   ├── LookupsController.cs
│   │   ├── RequestsController.cs
│   │   └── UsersController.cs
│   ├── Program.cs                     # Application entry point
│   └── appsettings.json              # Configuration
│
├── ApprovalWorkflow.Application/      # Application logic
│   ├── DTOs/                         # Data transfer objects
│   └── Interfaces/                   # Service interfaces
│
├── ApprovalWorkflow.Domain/          # Domain models
│   └── Entities/                     # Domain entities
│
├── ApprovalWorkflow.Infrastructure/  # Data access
│   └── Repositories/                 # Repository implementations
│
├── start.ps1                         # PowerShell startup script
├── BACKLOG.md                        # Feature backlog
└── LICENSE                           # Apache 2.0 License
```

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/forgot-password` - Request password reset

### Approval Requests
- `GET /api/approvalrequests` - Get all approval requests
- `GET /api/approvalrequests/{id}` - Get request by ID

### Requests Management
- `GET /api/requests` - Get all card requests
- `GET /api/requests/{id}` - Get request details
- `POST /api/requests` - Create new request
- `PUT /api/requests/{id}/approve` - Approve request
- `PUT /api/requests/{id}/reject` - Reject request

### User Management
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

### Lookups
- `GET /api/lookups/tiers` - Get all tiers
- `GET /api/lookups/approvers` - Get all approvers
- `GET /api/lookups/locations` - Get all locations
- `GET /api/lookups/roles` - Get all roles

### Data Management
- `GET /api/data-managers/locations` - Manage locations
- `GET /api/data-managers/statuses` - Manage statuses
- `GET /api/data-managers/roles` - Manage roles

For complete API documentation, run the application and visit `/swagger`.

## Contributing

Contributions are welcome! Here's how you can help:

### Reporting Issues

1. Check if the issue already exists in the [Issues](https://github.com/joedifrancesco-collab/ApprovalWorkflow/issues) section
2. If not, create a new issue with:
   - Clear title and description
   - Steps to reproduce (if applicable)
   - Expected vs actual behavior
   - Screenshots (if applicable)

### Submitting Changes

1. **Fork the Repository**
   ```bash
   # Click the "Fork" button on GitHub
   git clone https://github.com/YOUR-USERNAME/ApprovalWorkflow.git
   ```

2. **Create a Feature Branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Make Your Changes**
   - Write clean, maintainable code
   - Follow the existing code style and conventions
   - Add tests if applicable
   - Update documentation as needed

4. **Commit Your Changes**
   ```bash
   git add .
   git commit -m "Add: brief description of your changes"
   ```

5. **Push to Your Fork**
   ```bash
   git push origin feature/your-feature-name
   ```

6. **Submit a Pull Request**
   - Go to the original repository
   - Click "New Pull Request"
   - Select your fork and branch
   - Provide a clear description of your changes

### Development Guidelines

- Follow Clean Architecture principles
- Write meaningful commit messages
- Keep pull requests focused and small
- Ensure all tests pass before submitting
- Update documentation for new features
- Be respectful and constructive in code reviews

### Code Style

- Use C# naming conventions
- Follow .NET coding standards
- Use meaningful variable and method names
- Add XML comments for public APIs
- Keep methods focused and concise

## License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.

### Apache License 2.0

Copyright 2026 joedifrancesco-collab

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

---

## Acknowledgments

- Built with [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
- Developed as an experimental project with Claude Code
- Thanks to all contributors and the .NET community

## Support

For questions, issues, or suggestions:
- Open an [Issue](https://github.com/joedifrancesco-collab/ApprovalWorkflow/issues)
- Check the [BACKLOG.md](BACKLOG.md) for planned features
- Review existing documentation and code comments