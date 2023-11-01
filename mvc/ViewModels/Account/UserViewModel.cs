namespace mvc.ViewModels.Account;
using System.Text.Json.Serialization;

public class UserViewModel
{
    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("token")]
    public string Token { get; set; }
}
