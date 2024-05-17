# Cardo Web Shop

Welcome to the Cardo web shop repository! This README will guide you through the process of setting up the project on your local machine.

## Prerequisites

Before you begin, ensure you have met the following requirements:

- **SQL Server**: Make sure SQL Server is installed and running on your local machine. If you don't have it installed, you can download it from [Microsoft's website](https://www.microsoft.com/en-us/sql-server/sql-server-downloads).

- **Visual Studio 2022**: Download and install Visual Studio 2022 from [here](https://visualstudio.microsoft.com/downloads/).

- **.NET 8 SDK**: Ensure you have .NET 8 SDK installed. You can download it from [here](https://dotnet.microsoft.com/download/dotnet/8.0).

## Getting Started

1. **Clone the Repository**

   Clone the repository to your local machine using the following command:

   ```bash
   git clone https://github.com/TotalNodal/Cardo.git
   cd Cardo
2. **Update Configuration Files**

   Each microservice has an `appsettings.json` file that needs to be updated with your local SQL Server connection string. The microservices are located in the following directories:

   - `AuthAPI`
   - `CouponAPI`
   - `EmailAPI`
   - `OrderAPI`
   - `ProductAPI`
   - `ShoppingCartAPI`
   - `RewardAPI`

   Update the `appsettings.json` file for each microservice with your SQL Server connection string. Example:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=your_server_name;Database=your_database_name;User Id=your_username;Password=your_password;"
   }
3. **Run Database Migrations**

   For six of the microservices, you need to run the `add-migration` command. Follow these steps:

   - Open Visual Studio 2022.
   - Go to **Tools > NuGet Package Manager > Package Manager Console**.
   - In the Package Manager Console, select each project as the default project one by one and run the `add-migration` command. The six microservices that need migrations are:

     - `AuthAPI`
     - `CouponAPI`
     - `EmailAPI`
     - `OrderAPI`
     - `ProductAPI`
     - `ShoppingCartAPI`

     Example for `AuthAPI`:

     ```powershell
     PM> Select-Project AuthAPI
     PM> Add-Migration InitialCreate
     PM> Update-Database
     ```

     Repeat these steps for each of the six microservices.
4. **Configure Startup Projects**

   Configure the startup projects to include all seven microservices:

   - In Visual Studio, right-click on the solution in Solution Explorer and select **Set Startup Projects**.
   - Choose **Multiple startup projects**.
   - Set the Action to `Start` for the following projects:
     - `AuthAPI`
     - `CouponAPI`
     - `EmailAPI`
     - `OrderAPI`
     - `ProductAPI`
     - `ShoppingCartAPI`
     - `RewardAPI`
5. **Run the Solution**

   After configuring the startup projects, run the solution:

   - Click the Start button in Visual Studio to run all the configured microservices.

   This will start all seven microservices simultaneously, allowing them to work together as intended.
