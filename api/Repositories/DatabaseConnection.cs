using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Repositories;

public class DatabaseConnection : IdentityDbContext<UserModel>
{
    public DatabaseConnection(DbContextOptions options) : base(options)
    {
    }
    public DbSet<Book> Books { get; set; }
    public DbSet<Event> Events { get; set; }
}