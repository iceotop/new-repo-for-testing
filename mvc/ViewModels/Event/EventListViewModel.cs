using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.ViewModels.Event
{
    public class EventListViewModel
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Book { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}