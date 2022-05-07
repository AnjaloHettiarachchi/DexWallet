using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using DexWallet.Common.Clients;
using DexWallet.Common.Middlewares;
using DexWallet.Core.Contracts;
using DexWallet.Core.Services;

var builder = WebApplication.CreateBuilder(args);
// Add services to DI container
{
    var services = builder.Services;

    // Swagger
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

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
    // services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    // HTTP Client for inter-service communication
    services.AddHttpClient<IdentityServiceClient>();

    // Configure DI for application services
    services.AddTransient<IWalletService, WalletService>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseMiddleware<ResponseHandlerMiddleware>();

    app.UseMiddleware<RequestAuthorizationMiddleware>();

    app.MapControllers();
}

app.Run();