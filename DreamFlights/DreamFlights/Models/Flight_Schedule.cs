using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DreamFlights.Models
{
    public class Flight_Schedule
    {
        public int Flight_ScheduleID { get; set; }
        [Required]
        public int RouteID { get; set; }
        [Required]        
        public DateTime DepartDateTime { get; set; }
        [Required]
        public DateTime ArriveDateTime { get; set; }      
        public int AirCraftID { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than {0}")]
        public int Economy { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than {0}")]
        public int Business { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than {0}")]
        public int PremEconomy { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than {0}")]
        public int First { get; set; }
        [Required]
        public int NetFaire { get; set; }
        public string AirlineCode { get; set; }
        public Route Route { get; set; }
        public AirCraft AirCraft { get; set; }
    }
}
