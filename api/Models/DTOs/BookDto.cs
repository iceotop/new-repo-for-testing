namespace api.Controllers
{
    public partial class ExternalServicesController
    {
        public class BookDto
        {
            public string Title { get; set; }
            public string Author { get; set; }
            public string PublicationYear { get; set; }
            public string ImageUrl { get; set; }
        }
    }
}