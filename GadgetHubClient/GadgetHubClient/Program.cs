using Microsoft.EntityFrameworkCore;
using GadgetHubClient.Data;
using GadgetHubClient.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddControllers(); // Add API controller support

// Session requires a cache store; use in-memory cache for development
builder.Services.AddDistributedMemoryCache();

// Register HttpClient with SSL certificate handling for development
builder.Services.AddHttpClient("GadgetHubAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["GadgetHubAPI:BaseUrl"] ?? "https://localhost:7063");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    if (builder.Environment.IsDevelopment())
    {
        // Ignore SSL certificate errors in development
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
    }
    return handler;
});

// Register default HttpClient
builder.Services.AddHttpClient();

// Add DbContext
builder.Services.AddDbContext<GadgetHubDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Session Support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllers(); // Map API controllers
app.Run();
