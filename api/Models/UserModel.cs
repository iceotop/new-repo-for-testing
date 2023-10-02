using Microsoft.AspNetCore.Identity;

namespace api.Models;
public class UserModel : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}