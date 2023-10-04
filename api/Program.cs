using api.Data;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configurate database - Sqlite
builder.Services.AddDbContext<BookCircleContext>(
    options => options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"))
);

builder.Services.AddIdentityCore<UserModel>()
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<BookCircleContext>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed the database...
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<BookCircleContext>();
    await context.Database.MigrateAsync();

    //läs in i rätt ordning med hänsyn till beroende
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
