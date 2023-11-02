using System.Text;
using api.Data;
using api.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;
using Models;
using Repositories;
using Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:5120") // Replace with your frontend's address
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

builder.Services.AddHttpClient("GoogleBooks", c =>
    {
        c.BaseAddress = new Uri("https://www.googleapis.com/books/v1/");
    });

// Configurate database - Sqlite
builder.Services.AddDbContext<DatabaseConnection>(
    options => options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"))
);

// Add dependency injection
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IUserService, UserService>();

// Setup av JWT
builder.Services.AddIdentityCore<UserModel>()
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<DatabaseConnection>();
builder.Services.AddScoped<TokenService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Setup av JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("tokenSettings:tokenKey").Value))
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();

// Seed the database...
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<DatabaseConnection>();
    var userMgr = services.GetRequiredService<UserManager<UserModel>>();
    var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>();

    await context.Database.MigrateAsync();

    await SeedData.LoadRolesAndUsers(userMgr, roleMgr);
    await SeedData.LoadBooksData(context);
    await SeedData.LoadEventsData(context);
    
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    throw;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();