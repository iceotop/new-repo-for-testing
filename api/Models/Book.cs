using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

public class Book
{
    [Key]
    public string Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string PublicationYear { get; set; }
    public string Review { get; set; }
    public string ReadStatus { get; set; }
    public string ImageUrl { get; set; }


    // One-side
    public string? EventId { get; set; }
    [ForeignKey("EventId")]
    public Event? Event { get; set; }

    public string? UserModelId { get; set; }
    [ForeignKey("UserModelId")]
    public UserModel? User { get; set; }
}