using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data;
public class BookCircleContext : DbContext
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Event> Events { get; set; }
    // fixa users
    public BookCircleContext(DbContextOptions options) : base(options)
    {
    }
}