//This is a experimental class for many to many relation ship between transaction and passenger; May not be used in this project
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamFlights.Models
{
    public class Passenger_Transaction
    {
        public int TransactionID { get; set; }
        public int PassengerID { get; set; }
        public BookingTransaction BookingTransaction { get; set; }
        public Passenger passenger { get; set; }

    }
}
