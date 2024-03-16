using Initiate.Business;
using Initiate.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Initiate.Business.Providers;
using Initiate.Business.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Use SQLite or SQL Server based on your requirements
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Allows only specific type of requests in production for security
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy("AllowAll", builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
    }
    else
    {
        // Apply more restrictive CORS policy for production
        options.AddPolicy("AllowSpecific", builder =>
        {
            // Set up your specific CORS policy here
        });
    }
});

// Add ASP.NET Core Identity services
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure Identity cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    // Configure your cookie settings here for production security
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();

// Add Interfaces 

builder.Services.AddHostedService<NewsServiceHostedService>();

builder.Services.AddSingleton<INewsService>(provider =>
    new NewsService(provider.GetRequiredService<IServiceScopeFactory>()));

builder.Services.AddScoped<INewsRepository, NewsRepository>();
//builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPreferenceRepository, PreferenceRepository>();
builder.Services.AddScoped<IKeywordRepository, KeywordRepository>();
builder.Services.AddScoped<IFriendRepository, FriendRepository>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors(builder.Environment.IsDevelopment() ? "AllowAll" : "AllowSpecific");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
