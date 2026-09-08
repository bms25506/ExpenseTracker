# ExpenseTracker

ExpenseTracker is a personal finance web application built with ASP.NET Core MVC. It allows users to organize income and expenses into categories, manage financial transactions, and view a dashboard summarizing their current financial activity.

This project is being developed as a portfolio application to strengthen practical experience with C#, ASP.NET Core, Entity Framework Core, relational data modeling, validation, and CRUD application development.

## Project Status

ExpenseTracker is currently under active development.

The core application functionality is complete, including category management, transaction management, validation, database persistence, and a financial dashboard. Additional features such as transaction searching and filtering will be added as development continues.

## Features

### Dashboard

The dashboard provides an overview of financial activity, including:

- Total income
- Total expenses
- Current balance
- Five most recent transactions
- Quick links for adding and viewing transactions

### Transaction Management

Users can:

- View all transactions
- Add new transactions
- Edit existing transactions
- Delete transactions
- Assign transactions to categories
- Record transaction descriptions, amounts, and dates

Transactions are associated with either an Income or Expense category.

### Category Management

Users can:

- View categories
- Create categories
- Edit categories
- Delete unused categories
- Classify categories as Income or Expense

Categories that are currently being used by transactions cannot be deleted. This rule is enforced both by application logic and by the database relationship configuration.

### Validation

ExpenseTracker includes both client-side and server-side validation.

Examples include:

- Required transaction descriptions
- Required category names
- Positive transaction amounts
- Required category selection
- Maximum description and category-name lengths
- Database validation that verifies selected categories exist

## Technology Stack

- C#
- .NET 10
- ASP.NET Core MVC
- Entity Framework Core 10
- SQLite
- Razor Views
- LINQ
- Bootstrap
- JavaScript
- jQuery Validation
- Git
- GitHub

The application currently uses SQLite for local development.

The Entity Framework Core SQL Server provider is also included in the project to make a future migration to SQL Server or Azure SQL easier.

## Architecture

ExpenseTracker follows the ASP.NET Core MVC pattern.

```text
Browser
   |
   v
Controller
   |
   v
Entity Framework Core
   |
   v
SQLite Database
   |
   v
Controller
   |
   v
ViewModel / Model
   |
   v
Razor View
   |
   v
Browser
```

The project is organized into the following main areas:

```text
ExpenseTracker/
|
|-- Controllers/
|   |-- CategoriesController.cs
|   |-- HomeController.cs
|   `-- TransactionsController.cs
|
|-- Data/
|   `-- ApplicationDbContext.cs
|
|-- Migrations/
|
|-- Models/
|   |-- Category.cs
|   |-- ErrorViewModel.cs
|   `-- Transaction.cs
|
|-- ViewModels/
|   |-- DashboardViewModel.cs
|   `-- TransactionCreateViewModel.cs
|
|-- Views/
|   |-- Categories/
|   |-- Home/
|   |-- Shared/
|   `-- Transactions/
|
|-- wwwroot/
|
|-- Program.cs
|-- appsettings.json
`-- ExpenseTracker.csproj
```

## Data Model

ExpenseTracker currently uses two primary entities.

### Category

A category contains:

- Id
- Name
- Type

The `Type` value identifies the category as either:

- Income
- Expense

### Transaction

A transaction contains:

- Id
- Description
- Amount
- Date
- CategoryId

Each transaction belongs to one category.

The relationship is:

```text
Category
   1
   |
   |
   *
Transaction
```

One category can contain many transactions, while each transaction belongs to one category.

Category deletion is configured with restricted delete behavior so deleting a category does not automatically delete transaction history.

## Getting Started

### Prerequisites

Install the .NET 10 SDK.

Verify the installation with:

```powershell
dotnet --version
```

### Clone the Repository

```powershell
git clone https://github.com/bms25506/ExpenseTracker.git
```

Move into the project directory:

```powershell
cd ExpenseTracker
```

### Restore Dependencies

```powershell
dotnet restore
```

### Create the Local Database

The SQLite database itself is intentionally not stored in the Git repository.

Create the database from the Entity Framework Core migrations with:

```powershell
dotnet ef database update
```

If the Entity Framework Core command-line tools are not installed, install them with:

```powershell
dotnet tool install --global dotnet-ef
```

Then run:

```powershell
dotnet ef database update
```

### Run the Application

```powershell
dotnet run
```

Open the local address displayed in the terminal.

The application will normally provide routes such as:

```text
/                       Dashboard
/Transactions           Transaction history
/Transactions/Create    Add transaction
/Categories             Category management
/Categories/Create      Add category
```

## Database Migrations

Entity Framework Core migrations are included in source control so the database schema can be recreated without committing the local SQLite database file.

Current migrations include:

- Initial database creation
- Category validation metadata
- Restricted category deletion behavior

The local database file is excluded through `.gitignore`.

## Current Development Roadmap

Planned improvements include:

- Transaction search
- Category filtering
- Date-range filtering
- Additional dashboard improvements
- Financial summaries and reporting
- UI and responsive-design refinements
- Additional automated testing
- Deployment preparation
- Potential migration from SQLite to SQL Server or Azure SQL

## What This Project Demonstrates

This project is intended to demonstrate practical experience with:

- Building an ASP.NET Core MVC application
- Designing relational data models
- Creating one-to-many database relationships
- Using Entity Framework Core
- Creating and applying database migrations
- Performing asynchronous database operations
- Writing LINQ queries
- Implementing CRUD functionality
- Creating strongly typed ViewModels
- Using dependency injection
- Implementing client-side and server-side validation
- Protecting relational data integrity
- Using Git and GitHub for version control

## Author

Brianna Schneider