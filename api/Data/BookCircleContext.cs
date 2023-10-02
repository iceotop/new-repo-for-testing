using Microsoft.EntityFrameworkCore;

namespace api.Data;
public class BookCircleContext : DbContext
{
    public BookCircleContext(DbContextOptions options) : base(options)
    {
    }
}