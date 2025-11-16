using GadgetHubAPI.Data;
using GadgetHubAPI.Services;
using GadgetHubAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register HttpClient
builder.Services.AddHttpClient();

// Register Services
builder.Services.AddScoped<QuotationService>();
builder.Services.AddScoped<ProductSyncService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Register Repositories
builder.Services.AddScoped<ProductRepo>();
builder.Services.AddScoped<UserRepo>();
builder.Services.AddScoped<CustomerRepo>();
builder.Services.AddScoped<OrderRepo>();

var conn = builder.Configuration.GetConnectionString("cString");

// Register DbContext
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(conn));

// JWT Configuration
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

// CORS Configuration
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("dev", p => p
        .WithOrigins("http://localhost:5077", "http://localhost:3000", "http://localhost:5173", "http://localhost:7213", "https://localhost:7213", "https://localhost:7063")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("dev");
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
