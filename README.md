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

	``` dotnet restore InvestorFlowBanking.sln```

2. **Build the application**

	``` dotnet build InvestorFlowBanking.sln```

3. **Run the application**

	``` dotnet run --project IF.BankingAPI.csproj```

4. **Access the API**		

	![Swagger](/Images/Swagger.png)
   
   Open your browser and navigate to `https://localhost:7223/index.html` to view the API documentation.

### Run with Docker
1. **Build the Docker image**:
	```docker build -t if-banking-api .```
2. **Run the Docker container**:
	```docker run -d -p 8080:80 if-banking-api if-banking-api```
1. **Access the API**		
   Open your browser and navigate to `https://localhost:8080/swagger/index.html` to view the API documentation.

### API Documentation

#### Gateway API

The API is designed using DDD & Clean Architecture, where the core functionality exposed to end users is aviailable throught Gateway API.

![Gateway API](/Images/APIDoc_GatewayAPI.png)

Since this isa public gateway endpoint, you need not use any authentication to access the api.

##### Create Bank Account 
- **url**: `\api\banking\create`
- **Method**: `POST`
- **Description**: Creates a bank Account
- **curl**:

		curl --location '//api/Banking/create' \
			 --header 'Content-Type: application/json' \
			 --header 'Authorization: ••••••' \
			 --data '{
  						"customerId": "<uuid>",
  						"type": "Savings",
  						"creationDate": "<dateTime>"
					 }'

##### Delete Bank Account
- **url**: `\api\banking\delete`
- **Method**: `DELETE`
- **Description**: Deletes a bank Account
- **curl**:

		curl 	--location --request DELETE '//api/Banking/delete' \
				--header 'Content-Type: application/json' \
				--header 'Authorization: {{apiKey}}' \
				--data '{
							"customerId": "<uuid>"
						}'

##### Add Money
- **url**: `\api\banking\add-money`
- **Method**: `PUT`
- **Description**: Adds money to a bank account
- **curl**:

		curl --location --request PUT '//api/Banking/add-money' \
			--header 'Content-Type: application/json' \
			--header 'Authorization: {{apiKey}}' \
			--data '{
						"customerId": "<uuid>",
						"amount": "<double>",
						"currency": "<string>",
						"type": "Mortgage"
					}'

##### Withdraw Money
- **url**: `\api\banking\withdraw-money`
- **Method**: `PUT`
- **Description**: Withdraws money from a bank account
- **curl**:

		curl --location --request PUT '//api/Banking/withdraw-money' \
			--header 'Content-Type: application/json' \
			--header 'Authorization: {{apiKey}}' \
			--data '{
						"customerId": "<uuid>",
						"amount": "<double>",
						"currency": "<string>",
						"type": "Mortgage"
					}'

##### Transfer Funds
- **url**: `\api\banking\transfer-funds`
- **Method**: `PUT`
- **Description**: Transfers funds from one account to another
- **curl**:

		curl --location --request PUT '//api/Banking/transfer-funds' \
			--header 'Content-Type: application/json' \
			--header 'Authorization: ••••••' \
			--data '{
						"customerId": "<uuid>",
						"destinationAccountId": "<uuid>",
						"amount": "<double>",
						"currency": "<string>",
						"type": "Savings"
					}'

##### Transaction History
- **url**: `\api\banking\transaction-history`
- **Method**: `PUT`
- **Description**: Gets Transaction History associated to an account
- **curl**:

		curl --location --request GET '//api/Banking/transaction-history' \
				--header 'Content-Type: application/json' \
				--header 'Authorization: {{apiKey}}' \
				--data '{
							"customerId": "<uuid>"
						}'	

#### Admin API
The project offers admin access to deep dive into individual services/artifacts that are part of the Banking Workflow.

![Admin API](/Images/APIDoc_AdminAPI.png)

You will need to authenticate using Auth Endpoint inorder to access the API

**Login Credentials**: 

		UserName: admin	
		Password: admin

![Authentication](/Images/Admin_Access.png)

Copy the bearer token generated by the auth response and add it to the request headers by clicking on the Authorize button on the top right corner, and provide the token in the following format: ```bearer <token>```

![Authorization](/Images/AuthZ.png)

## Database

The application uses SQLite as the backend database. The database file is located at `IF.BankingAPI/Data/IFBanking.db`.

### Setup

*	Run the SetupDatabase console application to create the required tables needed for the application to run.

*	Once run, copy the banking.db file created in the bin folder to IF.BankingAPI folder and copy it in the Data folder.

*	Run the SQLite3 tool to browse through the tables as shown in the diagram below (SQLite3 is shipped with the repo)

*	Once all the tables are seen in the databse, you can run the solution in debug mode and use the gateway endpoints to perform banking operations.

![Sqlite3 Usage](/Images/SqliteTooling.png)

## Tests

### Integration Tests

*	Run integration tests using the command

	-	```cd IF.Infrastructure.IntegrationTests```
	-	```dotnet restore```
	-	```dotnet build```
	-	```dotnet test -v detailed```

**Note** Integration tests use a different database from the ones used by the actual solution. The database gets cleaned up after every test as part of the best unit test practices. The file is localed in the bin folder of the integration tests.

### Unit Tests

*	Run unit tests using the command
	
	-	```cd IF.Tests```
	-	```dotnet restore```
	-	```dotnet build```
	-	```dotnet test -v detailed```

**Note**: UnitTests are run using ```XUnit```, ```Moq``` and ```Fluent Assertions```

### Troubleshooting

#### Database

- If the database from the SetupDatabase doesn't work, you can as well use the default Database shipped with the repo. You can locate it in the IF.BankingAPI solution
- If you donot know the username and password to access the admin api, you can refer to them from ```AuthController``` ```Login``` Endpoint. The username is ```admin``` and password is ```admin```
- The postman collection of the endpoints generated by the solution are shared below
	* [Gateway API](https://api.postman.com/collections/5666963-b04ee64b-5095-4414-9520-5d33d436563a?access_key=<ask-for-key>)
	* [Admin API](https://api.postman.com/collections/5666963-41d363e8-890b-49e5-83b4-7df1adb99cf0?access_key=<ask-for-key>)

### Support

-	If things go south, feel free to reach out to me: 
-	Email: varunpvk@gmail.com




	
