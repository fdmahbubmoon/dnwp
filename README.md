# DNWP: Angular and .NET Core App with EF Core Code First Database

## Overview

This repository contains a sample application that integrates an Angular frontend with a .NET Core backend, utilizing Entity Framework (EF) Core for Code First database management. The combination of these technologies allows for the creation of a dynamic and interactive web application.

## Prerequisites

Before you begin, ensure you have the following installed:

- Node.js and npm for Angular development.
- .NET SDK for .NET Core development.
- Visual Studio or Visual Studio Code for .NET Core development (optional but recommended).

## Setup Instructions

1. **Clone the Repository:**

    ```bash
    git clone https://github.com/fdmahbubmoon/dnwp.git
    cd dnwp
    ```

2. **Angular App Setup:**

    ```bash
    cd ui
    npm install
    ```

3. **.NET Core App Setup:**

    ```bash
    cd DNWP.API
    dotnet restore
    ```

4. **Database Migration:**

    Update the connection string in `appsettings.Development.json` in the `DNWP.API` project. Make sure to select `DNWP.Infrastructure` as Default project in Package Manager Console. This will apply the initial database migration. Then run the following command in Package Manager Console:

   ```bash
    update-database    
    ```

6. **Run the Applications:**

    - Run the Angular app:

        ```bash
        cd ui
        ng serve
        ```

    - Run the .NET Core app using VS.

    Open your browser and navigate to `http://localhost:4200` to access the Angular app, and `https://localhost:7024` for the .NET Core app.
   Note that port numbers may be different

## Project Structure

- **UI:** Contains the Angular frontend code with HTML and SCSS.
- **DNWP:** Contains the .NET Core backend code.
  - **DNWP.API:** API endpoints and controllers.
  - **DNWP.Application:** Use cases and business logics.
  - **DNWP.Application.Tests:** Unit test project for DNWP.Application library.
  - **DNWP.Common:** Some common functionality such as Exceptions, helper classes etc.
  - **DNWP.Domain:** Models and entities.
  - **DNWP.Infrastructure:** External services as in database (Application DB context).
  - **DNWP.Repository:** Repository to execute command and queries in database.

## Contributing

This is a personal project with very specific requirements. Contributions are not recommended.
