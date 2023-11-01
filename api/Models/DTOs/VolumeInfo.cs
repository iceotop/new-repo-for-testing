namespace api.Controllers
{
    public partial class ExternalServicesController
    {
        public class VolumeInfo
        {
            public string Title { get; set; }
            public List<string> Authors { get; set; }
            public string PublishedDate { get; set; }
            public ImageLinks ImageLinks { get; set; }
        }
    }
}