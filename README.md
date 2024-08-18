# InvestorFlow Banking API
IF.BankingAPI is a sample ASP.NET Core Web API for managing banking operations.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (optional, for running in docker)
- [Postman](https://www.postman.com/downloads/) (optional, for testing the API)
- [Visual Studio Code](https://code.visualstudio.com/download) (optional, for editing the code)
- [Sqlite](https://www.sqlite.org/download.html)

## Getting Started

### Clone the repository

```git clone https://github.com/yourusername/IF.BankingAPI.git```

### Build and run the application locally

1. **Restore Dependencies**

	``` dotnet restore```

2. **Build the application**

	``` dotnet build```

3. **Run the application**

	``` dotnet run```

4. **Access the API**		
   
   Open your browser and navigate to `https://localhost:5001/swagger/index.html` to view the API documentation.

### Run with Docker
1. **Build the Docker image**:
	```docker build -t if-banking-api .```
2. **Run the Docker container**:
	```docker run -d -p 8080:80 if-banking-api if-banking-api```
1. **Access the API**		
   Open your browser and navigate to `https://localhost:8080/swagger/index.html` to view the API documentation.

## API Endpoints

- Account Management
	- GET /api/accounts
	- GET /api/accounts/{id}
	- POST /api/accounts
	- PUT /api/accounts/{id}
	- DELETE /api/accounts/{id}

- Vault Management
	- GET /api/vaults
	- GET /api/vaults/{id}
	- POST /api/vaults
	- PUT /api/vaults/{id}
	- DELETE /api/vaults/{id}
	- POST /api/vaults/{id}/deposit
	- POST /api/vaults/{id}/withdraw
	- POST /api/vaults/{id}/transfer

- Transaction Management
	- GET /api/transactions
	- GET /api/transactions/{id}
	- POST /api/transactions
	- PUT /api/transactions/{id}
	- DELETE /api/transactions/{id}

- BankAccount Management
	- GET /api/bankaccounts
	- GET /api/bankaccounts/{id}
	- POST /api/bankaccounts
	- PUT /api/bankaccounts/{id}
	- DELETE /api/bankaccounts/{id}
	- POST /api/bankaccounts/{id}/deposit
	- POST /api/bankaccounts/{id}/withdraw
	- POST /api/bankaccounts/{id}/transfer

## Database

The application uses SQLite as the database. The database file is located at `IF.BankingAPI/Data/IFBanking.db`.

### Troubleshooting




	