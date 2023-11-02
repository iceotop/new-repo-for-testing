using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Models;
using Repositories;

namespace api.Data;
public static class SeedData
{
    public static async Task LoadRolesAndUsers(UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager)
    {
        //Har vi inga roller så skapar vi två
        if (!roleManager.Roles.Any())
        {
            var admin = new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" };
            var user = new IdentityRole { Name = "User", NormalizedName = "USER" };

            await roleManager.CreateAsync(admin);
            await roleManager.CreateAsync(user);
        }

        //Har vi inga användare så skapar vi två
        //Skapar två användare
        if (!userManager.Users.Any())
        {
            var admin = new UserModel
            {
                UserName = "musses@mail.com",
                Email = "musses@mail.com",
                FirstName = "Musse",
                LastName = "Pigg",
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "User" });

            var user = new UserModel
            {
                UserName = "kalles@mail.com",
                Email = "kalles@mail.com",
                FirstName = "Kalle",
                LastName = "Anka",
            };

            await userManager.CreateAsync(user, "Pa$$w0rd");
            await userManager.AddToRoleAsync(user, "User");
        }
    }
    public static async Task LoadBooksData(DatabaseConnection databaseConnection)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        if (databaseConnection.Books.Any()) return;

        var json = File.ReadAllText("Data/json/books.json");

        var books = JsonSerializer.Deserialize<List<Book>>(json, options);

        if (books is not null && books.Count > 0)
        {
            await databaseConnection.Books.AddRangeAsync(books);
            await databaseConnection.SaveChangesAsync();
        }
    }
    public static async Task LoadEventsData(DatabaseConnection databaseConnection)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        if (databaseConnection.Events.Any()) return;

        var json = File.ReadAllText("Data/json/events.json");

        var events = JsonSerializer.Deserialize<List<Event>>(json, options);

        if (events is not null && events.Count > 0)
        {
            await databaseConnection.Events.AddRangeAsync(events);
            await databaseConnection.SaveChangesAsync();
        }

    }
}