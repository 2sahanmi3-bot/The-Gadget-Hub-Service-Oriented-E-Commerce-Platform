using Microsoft.EntityFrameworkCore;
using TechWorldAPI.Data;
using TechWorldAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Get connection string from appsettings.json
var conn = builder.Configuration.GetConnectionString("cString");

// Register DbContext with timeout configuration
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(conn, sqlOptions =>
    {
        sqlOptions.CommandTimeout(30);
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null);
    }));

// Register Repositories
builder.Services.AddScoped<ProductRepo>();

// Add AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add logging
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
