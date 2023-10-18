namespace mvc.ViewModels.Books;

public class BookBaseViewModel
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int PublicationYear { get; set; }
    public string Review { get; set; }
    public bool IsRead { get; set; }
}