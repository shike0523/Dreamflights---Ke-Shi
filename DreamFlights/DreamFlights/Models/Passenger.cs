using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DreamFlights.Models
{
    public enum Demographic
    {
        Adult, Youth, Child
    }
    public class Passenger
    {       
        public int PassengerID { get; set; }
        [Required]
        public string PersonalID { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Age { get; set; }
        public string Nationality { get; set; }
        public string Email { get; set; }
        public string Cellphone { get; set; }
        public Demographic? Demographic { get; set; }
    }
}
