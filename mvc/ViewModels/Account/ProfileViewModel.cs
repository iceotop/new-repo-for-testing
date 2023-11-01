
namespace mvc.ViewModels.Account
{
    public class ProfileViewModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Review { get; set; }
        public string ReadStatus { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<BookBaseViewModel>? Books { get; set; }
    }

}