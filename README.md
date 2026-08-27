<img width="184" height="679" alt="image" src="https://github.com/user-attachments/assets/c61540a0-06df-4740-8565-dcac243f850b" /># 💰 FinanceTracker

A personal finance management Web API built with **C# and ASP.NET Core**.

FinanceTracker allows users to manage their income and expenses, organize transactions by categories, and analyze their financial activity through statistics and reports.

The project is being developed as a portfolio project with a focus on clean architecture, REST API design, database management, authentication, and modern backend technologies.

---

## 🚀 Features

### 👤 User Management
- User registration
- Secure password hashing
- JWT authentication
- User-specific financial data

### 💳 Transactions
- Create income and expense transactions
- Edit transactions
- Delete transactions
- View transaction history
- Filter transactions by user and date

### 📂 Categories
- Create custom categories
- Separate income and expense categories
- Assign transactions to categories

### 📊 Financial Analytics
- Total income
- Total expenses
- Current balance
- Monthly statistics
- Expenses by category
- Most expensive categories

### 📄 Export & Reporting
- Export financial data to Excel
- Generate PDF reports

### ⚡ Performance
- Redis caching
- Optimized database queries

### 🐳 Deployment
- Docker support
- Containerized application
- PostgreSQL database

---

## 🛠️ Tech Stack

| Technology | Purpose |
|---|---|
| **C#** | Main programming language |
| **ASP.NET Core** | Web API framework |
| **Entity Framework Core** | ORM |
| **PostgreSQL** | Database |
| **JWT** | Authentication |
| **Redis** | Caching |
| **Swagger / OpenAPI** | API documentation |
| **Docker** | Containerization |
| **Git & GitHub** | Version control |

---

## 🏗️ Architecture

The project follows a layered architecture designed to separate business logic, application logic, infrastructure, and API responsibilities.


<img width="184" height="679" alt="image" src="https://github.com/user-attachments/assets/dad2c0ee-341e-48af-970e-108e912fc044" />


The current database contains the following main entities:

<img width="96" height="518" alt="image" src="https://github.com/user-attachments/assets/7b986565-dbf2-4705-924c-0334fa43d86c" />

The API is documented using Swagger / OpenAPI.

Current endpoints
Users
POST /api/Users/register

Register a new user.

Categories
POST /api/Categories

Create a financial category.

Transactions
POST /api/Transactions

Create a new transaction.

GET /api/Transactions/user/{userId}

Get transactions belonging to a specific user.

GET /api/Transactions/statistics/{userId}

Get financial statistics for a user.

💡 Example

Create an income transaction:

{
  "userId": "04100e0b-e3a2-4eb9-9495-b48d529f9af6",
  "categoryId": "ba557b66-5112-49ca-a07e-6b72cdf80260",
  "amount": 5000,
  "description": "Salary",
  "date": "2026-08-27T11:00:00Z",
  "type": 0
}

The API stores the transaction in PostgreSQL.

Example financial calculation:

Income:   5000 ₽
Expenses: 1700 ₽
----------------
Balance:  3300 ₽
⚙️ Getting Started
Prerequisites

Make sure you have installed:

.NET 8 SDK
PostgreSQL
Git
Clone the repository
git clone https://github.com/Vafobekov04/FinanceTracker.git
cd FinanceTracker
Configure the database

Create a PostgreSQL database:

CREATE DATABASE finance_tracker;

Configure the connection string using User Secrets or environment variables.

Example:

Host=localhost;
Port=5432;
Database=finance_tracker;
Username=postgres;
Password=YOUR_PASSWORD
Apply migrations
dotnet ef database update
Run the application
dotnet run

Swagger will be available at:

https://localhost:<port>/swagger
🔐 Security

The project is being developed with security in mind.

Planned security features include:

Password hashing
JWT authentication
Authorization
User-specific data access
Secure configuration management
Environment variables / User Secrets for sensitive data

⚠️ Sensitive credentials such as database passwords and JWT secrets should never be committed to the repository.

📈 Project Roadmap
 ASP.NET Core Web API setup
 PostgreSQL integration
 Entity Framework Core
 Database migrations
 User registration
 Category creation
 Transaction creation
 Transaction retrieval
 Basic financial statistics
 Password hashing
 JWT authentication
 Authorization
 Monthly statistics
 Expense analytics
 Filtering and pagination
 Redis caching
 Excel export
 PDF reports
 Docker
 Unit tests
 Integration tests
 CI/CD
📚 What This Project Demonstrates

This project demonstrates practical experience with:

RESTful API development
ASP.NET Core
C# backend development
Entity Framework Core
PostgreSQL
Relational database design
DTOs
Dependency Injection
Async programming
Database migrations
API documentation with Swagger
Authentication and authorization
Caching
Docker
Git and GitHub
👨‍💻 Author

Vafobek Vafobekov

Backend Developer | C# / ASP.NET Core
