using System.Text.Json;
using api.Models;

namespace api.Data;
public static class SeedData
{
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