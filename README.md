# Sales & Accounting System (.NET 10)

## Overview

A console-based business application that simulates a simplified ERP sales and accounting workflow.

The project represents a basic sales and accounting process including customer management, product catalog, quotations, orders, invoices, and payment tracking.

The main purpose of this project was to practice building a business application using C#, .NET, Entity Framework Core, SQL Server, and layered architecture.

The current version uses a CLI interface. REST API and web interface are planned as future improvements.

---

# Business Workflow

The application follows a simplified sales lifecycle:


Customer
↓
Quotation
↓
Order
↓
Invoice
↓
Payment


Documents are connected and follow business rules similar to real-world ERP systems.

---

# Features

## Customer Management

- Create customers
- Update customer data
- Delete customers
- Display customer list

## Product Management

- Create products
- Update product data
- Delete products
- Manage product catalog

## Quotation Management

- Create quotations
- Add quotation items
- Calculate totals
- Manage quotation statuses
- Convert accepted quotations into orders

## Order Management

- Create orders
- Manage order items
- Calculate order totals
- Track order lifecycle
- Convert completed orders into invoices

## Invoice Management

- Generate invoices from orders
- Calculate invoice totals
- Manage invoice statuses
- Track invoice payments

## Payment Management

- Register payments
- Assign payments to invoices
- Track payment status
- Monitor invoice settlement

---

# Technologies

- C#
- .NET 10
- Entity Framework Core
- SQL Server
- LINQ
- Dependency Injection
- Repository Pattern
- Unit of Work Pattern
- DTOs
- Mapping
- Fluent API
- Validation
- xUnit
- Moq

---

# Architecture

The project uses layered architecture.

Application flow:


UI
↓
Application Services
↓
Repositories
↓
Entity Framework Core
↓
SQL Server


---

# Project Structure


SalesAccountingSystem

├── AccountingSystem.Domain
│
├── AccountingSystem.Application
│
├── AccountingSystem.Infrastructure
│
├── AccountingSystem.UI
│
└── AccountingSystem.Tests


---

# Layers

## Domain

Contains core business models and rules.

Includes:

- Entities
- Enums
- Domain logic

Main entities:

- Customer
- Product
- Quotation
- Order
- Invoice
- Payment


## Application

Contains application logic and business workflows.

Includes:

- Services
- DTOs
- Interfaces
- Mappers
- Validators

This layer is independent from database implementation.


## Infrastructure

Responsible for external dependencies.

Includes:

- EF Core configuration
- DbContext
- Repository implementations
- Database migrations
- 
---

# Database

The project uses SQL Server with Entity Framework Core.

Implemented:

- Entity relationships
- Database constraints
- Fluent API configuration
- EF Core migrations


Main tables:

- Customers
- Products
- Quotations
- Orders
- Invoices
- Payments

---

# Design Decisions

Main architectural decisions:

- Separation of business logic from UI and database layers
- DTO usage to avoid exposing domain entities directly
- Repository pattern for database abstraction
- Unit of Work for transaction handling
- Services responsible for business workflows
- Dependency Injection for maintainability and testing
- Automated tests for application logic

---

# Getting Started

## Requirements

- .NET 10 SDK
- SQL Server / SQL Server Express


## Clone repository

```bash
git clone https://github.com/grzesiek1201/SalesAccountingSystem.git

cd SalesAccountingSystem
Apply migrations
dotnet ef database update
Run application
dotnet run
Example Workflow

Typical usage:

Create customer
Add products
Create quotation
Accept quotation
Convert quotation into order
Complete order
Generate invoice
Register payment
Tests

The project contains unit tests created with:

xUnit
Moq

Covered areas:

Services
Validators
Business scenarios
Learning Goals

This project helped practice:

Object-Oriented Programming
SOLID principles
Clean Code practices
Layered architecture
Entity Framework Core
SQL Server
Database migrations
Repository Pattern
Dependency Injection
Business process modeling
Unit testing
Planned Improvements

Future development:

Logging with Serilog
Authentication and authorization
User roles and permissions
REST API with ASP.NET Core
Swagger documentation
Reporting module
Web frontend
Docker support
CI/CD pipeline
Future Direction

The long-term goal is to evolve the application into a small business management system containing:

Web API
User management
Role-based access control
Reporting
Web interface
