using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Repositories;

public class DatabaseConnection : IdentityDbContext<UserModel>
{
    private const string CONNECTION_STRING = "Data Source=bookCircle.db";
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(CONNECTION_STRING);

        base.OnConfiguring(optionsBuilder);
    }
    public DbSet<Book> Books { get; set; }
    public DbSet<Event> Events { get; set; }
}