namespace api.ViewModels;

public class EventBaseViewModel
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Book { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ICollection<BookBaseViewModel>? Books { get; set; }
}