using System.Text.Json;
using api.Models;
using Microsoft.AspNetCore.Identity;

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
                UserName = "daniels@mail.com",
                Email = "daniels@mail.com",
                FirstName = "Daniel",
                LastName = "Hertz",
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
    public static async Task LoadBooksData(BookCircleContext context)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        if (context.Books.Any()) return;

        var json = File.ReadAllText("Data/json/books.json");

        var books = JsonSerializer.Deserialize<List<Book>>(json, options);

        if (books is not null && books.Count > 0)
        {
            await context.Books.AddRangeAsync(books);
            await context.SaveChangesAsync();
        }
    }
    public static async Task LoadEventsData(BookCircleContext context)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        if (context.Events.Any()) return;

        var json = File.ReadAllText("Data/json/events.json");

        var events = JsonSerializer.Deserialize<List<Event>>(json, options);

        if (events is not null && events.Count > 0)
        {
            await context.Events.AddRangeAsync(events);
            await context.SaveChangesAsync();
        }

    }
}