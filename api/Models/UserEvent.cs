using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class UserEvent
    {
        [Key]
        public string UserId { get; set; }
        public UserModel UserModel { get; set; }

        public string EventId { get; set; }
        public Event Event { get; set; }

    }
}