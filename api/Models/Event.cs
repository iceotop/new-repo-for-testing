using System.ComponentModel.DataAnnotations;

namespace api.Models;
public class Event
{
    [Key]
    public string Id { get; set; }
    public string Title { get; set; }
    public string Book { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public ICollection<UserEvent> UserEvents { get; set; }

    // Many-side
    public ICollection<Book>? Books { get; set; }
}