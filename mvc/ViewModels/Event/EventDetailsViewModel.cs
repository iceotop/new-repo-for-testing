using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.ViewModels.Event;

    public class EventDetailsViewModel
    {
        [Required(ErrorMessage = "Id är obligatoriskt")]
        [DisplayName("Id")]
        public string Id { get; set; } = "";

        [Required(ErrorMessage = "Title är obligatoriskt")]
        [DisplayName("Title")]
        public string Title { get; set; } = "";

        [Required(ErrorMessage = "Book är obligatoriskt")]
        [DisplayName("Book")]
        public string Book { get; set; } = "";

        [Required(ErrorMessage = "Description är obligatoriskt")]
        [DisplayName("Description")]
        public string Description { get; set; } = "";

        [DisplayName("StartDate")]
        public string StartDate { get; set; } = "";

        [DisplayName("EndDate")]
        public string EndDate { get; set; } = "";
    }
