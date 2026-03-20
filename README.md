# NIB Insurance Management System API

## Overview

**NIB Insurance Management System** is a backend Web API built using ASP.NET Core to digitize and streamline insurance operations.
It replaces inefficient manual processes with a fast, accurate, and scalable system for managing insurance workflows.

The system is designed for insurance companies and supports multiple roles including **Admin, Client, Operator, Finance, and Manager**.

---

## Problem Statement

Traditional insurance systems rely on manual processes which are:

* Slow and time-consuming
* Prone to human error
* Difficult to scale and manage

This project solves these issues by providing a centralized, automated, and role-based system.

---

## Tech Stack

* **Backend:** ASP.NET Core Web API (.NET)
* **Database:** PostgreSQL
* **ORM:** Entity Framework Core
* **Authentication:** JWT (JSON Web Token)
* **API Testing:** Swagger

---

## Architecture

This project follows a **Layered Architecture** approach:

```
API → Application → Domain → Infrastructure
```

### Key Components:

* DTOs (Data Transfer Objects)
* Service Layer (Business Logic)
* Repository Pattern (via EF Core)
* Global Exception Handling Middleware

---

## Features

* 🔐 User Registration & Login (JWT Authentication)
* 👥 Role-Based Authorization (Admin, Client, Operator, Finance, Manager)
* 📄 Policy Management System
* 📑 Claim Management System
* 🧑‍💼 Client Management
* 💰 Payment Handling
* 📁 File Upload System
* 📊 Accurate Policy Calculations
* 📢 News & Announcement System
* 🧾 Structured and Scalable API Design

---

## API Modules

The system is organized into multiple controllers, including:

* Auth Controller
* User Controller
* Policy Controller
* Claim Controller
* Payment Controller
* Announcement Controller

---

## Database Design

The system includes **13+ entities**, such as:

* User
* Policy
* Claim
* Payment
* (and additional related entities)

---

## Getting Started

### Prerequisites

* .NET SDK installed
* PostgreSQL installed
* Visual Studio or VS Code

---

### Setup Instructions

1. Clone the repository:

```
git clone https://github.com/Leul-ab/NIB-Insurance.git
```

2. Navigate to the project folder and open it in Visual Studio

3. Configure the database:

   * Open `appsettings.json`
   * Update database name and password
   * Add company email and app specific password

4. Apply migrations:

```
Add-Migration InitialCreate
Update-Database
```

5. Run the project:

```
dotnet run
```

6. Open Swagger:

```
https://localhost:<port>/swagger
```

---

## 📸 Screenshots

> (Add your Swagger / Postman screenshots here)

* Swagger UI
* API Endpoints Testing
* Sample Requests & Responses

---

## Authentication

This project uses **JWT-based authentication** to secure endpoints and manage user roles.

---

## Future Improvements

* Add Unit Testing
* Implement Logging System
* Add Pagination & Filtering Enhancements
* Deploy to Cloud (Azure / Render)

---

## 👨‍💻 Author

Developed as part of a backend engineering project using modern .NET practices and clean architecture principles.

---

## Final Note

This project demonstrates:

* Real-world backend development skills
* Clean architecture implementation
* Secure authentication & authorization
* Scalable API design

If you find this project useful, feel free to ⭐ the repository!
