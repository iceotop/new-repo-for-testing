using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace mvc.ViewModels.Account;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Användarnamn Saknas")]
    // [EmailAddress(ErrorMessage = "Felaktigt Användarnamn")]
    [DisplayName("Användarnamn")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Lösenord saknas")]
    [DisplayName("Lösenord")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Required(ErrorMessage = "E-post saknas")]
    [EmailAddress(ErrorMessage = "Felaktig E-postadress")]
    [DisplayName("E-post")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Förnamn Saknas")]
    [DisplayName("Förnamn")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Efternamn Saknas")]
    [DisplayName("Efternamn")]
    public string? LastName { get; set; }
}