using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using DexWallet.Common.Clients;
using DexWallet.Common.Helpers;
using DexWallet.Common.Middlewares;
using DexWallet.Exchange.Contracts;
using DexWallet.Exchange.Entities.Models;
using DexWallet.Exchange.Services;

var builder = WebApplication.CreateBuilder(args);
// Add services to DI container
{
    var services = builder.Services;

    // Swagger
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    // Health checks for ECS
    services.AddHealthChecks();

    // JSON default options
    services.AddControllers().AddJsonOptions(
        options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        }
    );

    // AWS Profile from Configuration providers
    var awsOptions = builder.Configuration.GetAWSOptions();
    services.AddDefaultAWSOptions(awsOptions);

    // AWS Services
    services.AddAWSService<IAmazonDynamoDB>();
    services.AddScoped<IDynamoDBContext, DynamoDBContext>();

    // Configure strongly typed Configuration object
    services.Configure<CommonAppSettings>(builder.Configuration.GetSection("AppSettings"));

    // HTTP Client for inter-service communication
    services.AddHttpClient<IdentityServiceClient>();
    services.AddHttpClient<CoreServiceClient>();

    // Configure DI for application services
    services.AddTransient<IRateService, RateService>();
    services.AddTransient<IExchangeService, ExchangeService>();
}

var app = builder.Build();

// Migrate mock user to Database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<IDynamoDBContext>();

    var rateBatch = context.CreateBatchWrite<Rate>();

    rateBatch.AddPutItems(new List<Rate>
    {
        new() { RegularType = "USD", CryptoType = "BTC", Value = 0.000027m },
        new() { RegularType = "EUR", CryptoType = "BTC", Value = 0.000029m },
        new() { RegularType = "USD", CryptoType = "ETH", Value = 0.000347196m },
        new() { RegularType = "EUR", CryptoType = "ETH", Value = 0.000380662m },
        new() { RegularType = "USD", CryptoType = "BNB", Value = 0.0027m },
        new() { RegularType = "EUR", CryptoType = "BNB", Value = 0.0028m },
        new() { RegularType = "USD", CryptoType = "SOL", Value = 3.79m },
        new() { RegularType = "EUR", CryptoType = "SOL", Value = 3.98m }
    });

    var ratesInDb = context.ScanAsync<Rate>(new List<ScanCondition>()).GetRemainingAsync().Result;

    if (ratesInDb.Count == 0) rateBatch.ExecuteAsync();
}

// Configure the HTTP request pipeline.
{
    // Health check endpoint
    app.UseHealthChecks("/health");

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // app.UseHttpsRedirection();

    app.UseMiddleware<ResponseHandlerMiddleware>();

    app.UseMiddleware<RequestAuthorizationMiddleware>();

    app.MapControllers();
}

app.Run();