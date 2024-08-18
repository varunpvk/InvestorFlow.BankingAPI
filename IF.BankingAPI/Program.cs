using IF.Application.Abstractions;
using IF.Application.AccountService.CommandHandlers;
using IF.Application.AccountService.Commands;
using IF.Application.AccountService.Queries;
using IF.Application.AccountService.QueryHandler;
using IF.Application.BankingService.CommandHandlers;
using IF.Application.BankingService.Commands;
using IF.Application.BankingService.Queries;
using IF.Application.BankingService.QueryHandler;
using IF.Application.TokenService;
using IF.Application.TransactionService.CommandHandlers;
using IF.Application.TransactionService.Commands;
using IF.Application.TransactionService.Queries;
using IF.Application.TransactionService.QueryHandler;
using IF.Application.VaultService.CommandHandlers;
using IF.Application.VaultService.Commands;
using IF.Application.VaultService.Queries;
using IF.Application.VaultService.QueryHandler;
using IF.Domain;
using IF.Domain.DTOs;
using IF.Domain.ErrorMessages;
using IF.Infrastructure;
using IF.Infrastructure.BankingRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Data.Sqlite;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Data;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/investorfllow-logs.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog(); // add this line to use serilog

// Add services to the container.
var configuration = builder.Configuration;

builder.Services.AddSingleton<BankingContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.IncludeFields = true;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter into field the word 'Bearer' following by space and JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });

    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

        var groupName = methodInfo.DeclaringType
            .GetCustomAttributes(true)
            .OfType<ApiExplorerSettingsAttribute>()
            .Select(attr => attr.GroupName)
            .FirstOrDefault();

        return groupName == docName;
    });

    c.SwaggerDoc("InvestorFlow Banking", new OpenApiInfo { Title = "InvestorFlow Banking Gateway API", Version = "v1" });
    c.SwaggerDoc("Banking API", new OpenApiInfo { Title = "Banking Admin API", Version = "v1" });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IDbConnection>(sp =>
{
    var connection = new SqliteConnection("DataSource=Banking.db");
    connection.Open();
    return connection;
});

// AccountService Registration
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ICommandHandler<CreateAccountCommand, Result<bool, ValidationError>>, CreateAccountCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateAccountCommand, Result<bool, ValidationError>>, UpdateAccountCommandHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteAccountCommand, Result<bool, ValidationError>>, DeleteAccountCommandHandler>();
builder.Services.AddScoped<IQueryHandler<GetAccountByIdQuery, Result<AccountDTO, NotFoundError>>, GetAccountByIdQueryHandler>();

// Vault Service Registration
builder.Services.AddScoped<IVaultRepository, VaultRepository>();
builder.Services.AddScoped<ICommandHandler<CreateVaultCommand, Result<bool, ValidationError>>, CreateVaultCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateVaultCommand, Result<bool, ValidationError>>, UpdateVaultCommandHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteVaultCommand, Result<bool, ValidationError>>, DeleteVaultCommandHandler>();
builder.Services.AddScoped<IQueryHandler<GetVaultByIdQuery, Result<VaultDTO, NotFoundError>>, GetVaultByIdQueryHandler>();

// Transaction Service Registration
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ICommandHandler<CreateTransactionCommand, Result<bool, ValidationError>>, CreateTransactionCommandHandler>();
builder.Services.AddScoped<IQueryHandler<GetTransactionByIdQuery, Result<TransactionDTO, NotFoundError>>, GetTransactionByIdQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetTransactionsByAccountIdQuery, Result<IList<TransactionDTO>, NotFoundError>>, GetTransactionsByAccountIdQueryHandler>();

// Banking Service Registration
builder.Services.AddScoped<ICustomerAccountRepository, CustomerAccountRepository>();
builder.Services.AddScoped<ICommandHandler<CreateBankAccountCommand, Result<bool, ValidationError>>, CreateBankAccountCommandHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteBankAccountCommand, Result<bool, ValidationError>>, DeleteBankAccountCommandHandler>();
builder.Services.AddScoped<ICommandHandler<AddMoneyCommand, Result<bool, ValidationError>>, AddMoneyCommandHandler>();
builder.Services.AddScoped<ICommandHandler<WithdrawMoneyCommand, Result<bool, ValidationError>>, WithdrawMoneyCommandHandler>();
builder.Services.AddScoped<ICommandHandler<TransferFundsCommand, Result<bool, ValidationError>>, TransferFundsCommandHandler>();
builder.Services.AddScoped<IQueryHandler<GetTransactionHistoryQuery, Result<IDictionary<Guid, IList<TransactionDTO>>, NotFoundError>>, GetTransactionHistoryQueryHandler>();


builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<TokenCreator>();

var jwtKey = builder.Configuration.GetSection("Secrets")["JwtKey"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "iss",
            ValidAudience = "aud",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/InvestorFlow Banking/swagger.json", "InvestorFlow Banking Gateway API v1");
    c.SwaggerEndpoint("/swagger/Banking API/swagger.json", "Banking Admin API V1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});

try
{
    Log.Information("Starting InvestorFlow Banking API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}
app.MapControllers();



