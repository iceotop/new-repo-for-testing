using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace mvc.ViewModels.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "Användarnamn krävs")]
    [DisplayName("Användarnamn")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Lösenord krävs")]
    [DisplayName("Lösenord")]
    public string Password { get; set; }
}