using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace mvc.Models
{
    public class UserModel : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public ICollection<EventModel> Events { get; set; } = new List<EventModel>();
        // public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}