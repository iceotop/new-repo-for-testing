namespace api.Controllers
{
    public partial class ExternalServicesController
    {
        //------classes to handle response from google books api
        public class GoogleBooksResponse
        {
            public List<GoogleBookItem> Items { get; set; }
        }
    }
}