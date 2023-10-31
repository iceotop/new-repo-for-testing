using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace api.Data;
public class BookCircleContext : IdentityDbContext<UserModel>
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Event> Events { get; set; }
    public BookCircleContext(DbContextOptions options) : base(options)
    {
    }
}