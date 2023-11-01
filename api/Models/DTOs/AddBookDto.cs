using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.DTOs
{
    public class AddBookDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string PublicationYear { get; set; }
        public string Review { get; set; } = "";
        public string ReadStatus { get; set; }
        public string ImageUrl { get; set; }
    }
}