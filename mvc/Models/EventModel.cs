using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models;

    public class EventModel
    {
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Book { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ICollection<UserModel> Users { get; set; } = new List<UserModel>();
    
    }
