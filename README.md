

# ADEEB

Welcome to the adeeb repository! This guide will help you set up and run the project using Docker and .NET.

## Prerequisites

Before you begin, make sure you have the following installed on your system:

1. **Docker**  
   - Download and install Docker from the official [Docker website](https://www.docker.com/products/docker-desktop).
   - Verify the installation by running the following command in your terminal or command prompt:
     ```bash
     docker --version
     ```

2. **Docker Compose**  
   - Docker Compose is included with Docker Desktop. Verify the installation:
     ```bash
     docker-compose --version
     ```

3. **.NET SDK**  
   - Download and install the .NET SDK from the official [.NET website](https://dotnet.microsoft.com/).
   - Verify the installation:
     ```bash
     dotnet --version
     ```

## Setting Up the Project

### Step 1: Clone the Repository
Clone this repository to your local machine:
```bash
git clone https://github.com/yourusername/your-repository-name.git
cd your-repository-name
```

### Step 2: Set Up Docker

1. Navigate to the root directory containing the `docker-compose.yml` file.
2. Start the Docker containers:
   ```bash
   docker-compose up -d
   ```
   This will:
   - Start a MySQL container.
   - Start a phpMyAdmin container for managing the database.

3. Access phpMyAdmin in your browser:
   - URL: `http://localhost:8080`
   - Username: `root`
   - Password: `keepitsecret`

### Step 3: Configure the .NET Application

1. Update the connection string in the `appsettings.json` file to match the MySQL container:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost,3306;Database=mysqladeeb;User Id=root;Password=keepitsecret;"
     }
   }
   ```

2. Apply migrations to set up the database schema:
   ```bash
   dotnet ef database update
   ```

### Step 4: Run the .NET Application

Start the .NET application:
```bash
dotnet run
```

The application will run on `http://localhost:5000` by default (or `https://localhost:5001` for HTTPS).

## Troubleshooting

- If you encounter any issues with Docker containers:
  ```bash
  docker-compose logs
  ```
- Ensure that no other application is using the ports `3306` or `8080`.



## Contributing

Please create new branch for this repository, make your changes, and submit a pull request.

USE (NAME/TASK)

---


```
