using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Models;
public class UserModel : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public ICollection<Event> Events { get; set; } = new List<Event>();
    public ICollection<Book> Books { get; set; } = new List<Book>();
}