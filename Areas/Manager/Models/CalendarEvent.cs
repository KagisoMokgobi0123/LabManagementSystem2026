using System;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class CalendarEvent
    {
        [Key]
        public int CalenderId { get; set; }
        public string Title { get; set; }
        public DateTime EventDate { get; set; }
        public string Color { get; set; } = "primary";
    }
}
