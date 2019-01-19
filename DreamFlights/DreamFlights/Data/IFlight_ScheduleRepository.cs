using DreamFlights.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamFlights.Data
{
    public interface IFlight_ScheduleRepository : IDisposable
    {
        Task<List<Passenger>> CreatPassengers(string[] lastName, string[] firstName, string[] ID);
        Task UpdateSeatsAsync(List<int> tripList, string cabin, int passangers, string userName, List<Passenger> passengersList, int suspendedID);
        int InitialBooking(List<int> tripList, string cabin, int passangers, DateTime bookingDateTime);
        Task SaveAsync();
    }
}
