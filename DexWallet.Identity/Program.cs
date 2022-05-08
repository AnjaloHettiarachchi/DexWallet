using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using DexWallet.Common.Middlewares;
using DexWallet.Identity.Contracts;
using DexWallet.Identity.Entities.Models;
using DexWallet.Identity.Helpers;
using DexWallet.Identity.Middlewares;
using DexWallet.Identity.Services;

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
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    // Configure DI for application services
    services.AddTransient<IJwtUtilities, JwtUtilities>();
    services.AddTransient<IUserService, UserService>();
}

var app = builder.Build();

// Migrate mock user to Database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<IDynamoDBContext>();

    var userBatch = context.CreateBatchWrite<User>();

    userBatch.AddPutItems(new List<User>
    {
        new() { FirstName = "John", LastName = "Doe", Username = "john.doe@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("john123") },
        new() { FirstName = "Jane", LastName = "Doe", Username = "jane.doe@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("jane123") }
    });

    var usersInDb = context.ScanAsync<User>(new List<ScanCondition>()).GetRemainingAsync().Result;

    if (usersInDb.Count == 0) userBatch.ExecuteAsync();
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

    app.UseMiddleware<JwtMiddleware>();

    app.MapControllers();
}

app.Run();