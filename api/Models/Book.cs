using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

public class Book
{
    [Key]
    public string Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public int PublicationYear { get; set; }
    public string Review { get; set; }
    public bool IsRead { get; set; }


    // One-side
    public string? EventId { get; set; }
    [ForeignKey("EventId")]
    public Event? Event { get; set; }
}