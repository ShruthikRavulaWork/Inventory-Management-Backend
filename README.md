# Inventory Management API

This repository contains the backend source code for the Inventory & Supplier Management application. It is a RESTful API built with .NET 8, designed to handle all business logic, data persistence, and security for the system.

## Features

-   **Secure Authentication:** JWT-based authentication for user login and registration.
-   **Role-Based Authorization:** Clear separation of permissions between 'Admin' and 'Supplier' roles.
-   **Full CRUD Functionality:** Complete Create, Read, Update, and Delete operations for inventory items.
-   **Soft Deletion:** Items are never permanently deleted from the database, ensuring data history is preserved. An `is_deleted` flag is used to mark items as inactive.
-   **Supplier-Specific Endpoints:** Suppliers can only view and modify the price/quantity of items assigned to them.
-   **Analytics Endpoints:** Dedicated endpoints to provide aggregated data for admin dashboard charts.
-   **Structured Exception Handling:** A global middleware catches all unhandled exceptions, logs them, and returns a user-friendly error message, preventing application crashes.
-   **Unit Tested:** Key business logic in controllers is covered by unit tests to ensure reliability.
-   **API Documentation:** Integrated Swagger/OpenAPI support for easy testing and exploration of endpoints.

## Technology Stack

-   **Framework:** .NET 8
-   **Database:** SQL Server
-   **Authentication:** JSON Web Tokens (JWT)
-   **API Documentation:** Swashbuckle (Swagger)

---

## Prerequisites

Before you begin, ensure you have the following software installed on your machine:

-   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
-   [SQL Server 2021 or later](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express Edition is sufficient)
-   A database management tool like [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms).
-   An API client like [Postman](https://www.postman.com/downloads/).

---

## Setup and Installation

Follow these steps to get the backend running locally.

### 1. Database Setup

1.  **Create the Database:**
    -   Open SSMS or your preferred database tool and connect to your SQL Server instance.
    -   Create a new, empty database named `InventoryDB`.

2.  **Run the SQL Scripts:**
    -   Execute the provided `db-setup.sql` script against the `InventoryDB`.
      
---

### 2. Cloning the Repository

To get started, clone the repository:

```bash
git clone https://github.com/your-username/Inventory-Management-Backend.git
```
- **To navigate to the Backend directory from your current folder:**
```bash
cd Inventory-Management-Backend/Inventory-Management
```
- **TO navigate to unit-tests directory from your current folder:**
```bash
cd Inventory-Management-Backend/tests/InventoryAPI.Tests
```
---
### 3. API Configuration

1.  **Navigate to the Backend Directory:**
    -   Open a terminal or command prompt and navigate to the backend directory.

2.  **Configure Connection String:**
    -   Open the `appsettings.json` file.
    -   Locate the `ConnectionStrings` section and update the `DefaultConnection` value with your specific SQL Server credentials. Replace `SERVER_NAME` with actual server name.
        ```json
        "ConnectionStrings": {
          "DefaultConnection": "Server=[SERVER_NAME];Database=InventoryDB;TrustServerCertificate=True;"
        }
        ```

3.  **Configure JWT Secret:**
    -   In the same `appsettings.json` file, find the `Jwt` section.
    -   Replace the value of `Key` with your own long, secret, and randomly generated string. This is critical for security.
        ```json
        "Jwt": {
          "Key": "SECRET_AND_LONG_KEY_FOR_JWT_SIGNING",
          "Issuer": "https://localhost:7001",
          "Audience": "https://localhost:7001"
        }
        ```
---

### 4. Running the Application

1.  **Restore Dependencies:**
    -   In your terminal (still in the `Inventory-Management` directory), run the following command:
        ```bash
        dotnet restore
        ```

2.  **Run the API:**
    -   Execute the run command:
        ```bash
        dotnet run
        ```
    -   The API will start and listen on `http://localhost:5268` (the port may vary).

### 5. Creating the Initial Admin User

Since new users are registered as 'Supplier' by default, you must use a special endpoint to create your first 'Admin' user.

1.  Open the Postman Application.
2.  Make a `POST` request to `http://localhost:5268/api/Auth/create-admin` endpoint.
3.  In the request body, enter the credentials for your admin user:
    ```json
    {
      "username": "admin",
      "password": "A_Strong_Password!123"
    }
    ```
3.  Click "Execute". You should receive a `200 OK` response. You can now log in with these credentials from the frontend.

---

## Running the Unit Tests

The solution includes a separate project for unit tests to ensure the controllers behave as expected.

1.  **Navigate to the Test Project Directory:**
    -   Open a new terminal and navigate to the unit-tests project:


2.  **Restore Dependencies:**
    ```bash
    dotnet restore
    ```

3.  **Run Tests:**
    -   Execute the test command:
        ```bash
        dotnet test
        ```
    -   The results of the tests will be displayed in the terminal. You can also run them using the Test Explorer in Visual Studio.
