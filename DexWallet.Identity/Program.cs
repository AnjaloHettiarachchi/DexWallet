using System.Text.Json;
using System.Text.Json.Serialization;
using DexWallet.Common.Middlewares;
using DexWallet.Identity.Contracts;
using DexWallet.Identity.Entities.Models;
using DexWallet.Identity.Helpers;
using DexWallet.Identity.Middlewares;
using DexWallet.Identity.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to DI container
{
    var services = builder.Services;

    // Swagger
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    // EF Core Database Context
    services.AddDbContext<DataContext>(optionsBuilder => { optionsBuilder.UseInMemoryDatabase("TestDB"); });

    // JSON default options
    services.AddControllers().AddJsonOptions(
        options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        }
    );

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
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    var testUser = new User
    {
        FirstName = "John",
        LastName = "Doe",
        Username = "john.doe@example.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("john123")
    };
    context.Users.Add(testUser);
    context.SaveChanges();
}

// Configure the HTTP request pipeline.
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseMiddleware<ResponseHandlerMiddleware>();

    app.UseMiddleware<JwtMiddleware>();

    app.MapControllers();
}

app.Run();