using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DreamFlights.Models
{
    public enum CabinType
    {
        Economy, Business, PremEconomy, First
    }
    public class BookingTransaction
    {
        public int BookingTransactionID { get; set; }
        [Required]
        public DateTime BookingDateTime { get; set; }
        [Required]
        public DateTime DepartDateTime { get; set; }
        [Required]
        public int Flight_ScheduleID { get; set; }
        public string PersonalID { get; set; }
        //public Passenger Passenger { get; set; }
        public CabinType? CabinType { get; set; }
        public Flight_Schedule Flight_Schedule { get; set; }
        public int SuspendedID { get; set; }
        public bool Suspended { get; set; }
        public string CustomerEmail { get; set; }
    }
}
