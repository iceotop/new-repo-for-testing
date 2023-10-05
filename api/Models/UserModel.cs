using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace api.Models;
public class UserModel : IdentityUser
{
    [Key]
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    public ICollection<UserEvent> UserEvents { get; set; }
}