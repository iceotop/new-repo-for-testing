using System.ComponentModel.DataAnnotations;

namespace Models;
public class Event
{
    [Key]
    public string Id { get; set; }
    public string Title { get; set; }
    public string Book { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public ICollection<UserModel> Users { get; set; } = new List<UserModel>();

    // Many-side
    public ICollection<Book>? Books { get; set; }
}