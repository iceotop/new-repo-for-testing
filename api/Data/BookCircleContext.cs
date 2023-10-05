using api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data;
public class BookCircleContext : IdentityDbContext<UserModel>
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Event> Events { get; set; }
    // fixa users
    public BookCircleContext(DbContextOptions options) : base(options)
    {
    }
}