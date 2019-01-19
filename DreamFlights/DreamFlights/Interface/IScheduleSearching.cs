using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamFlights.Interface
{
    public interface IScheduleSearching
    {
        Task<List<CandidateTrip>> SearchAsync(int FromCityID, int ToCityID, string takeoffDate, int adults, int Youths, int Children, string cabin);
        Task SearchSchedules(int FromCityID, int ToCityID, string trip, int searchCount, DateTime NextEarliestDepartLimit);
        void AddCities(List<string> stopList, List<int> routeNumbers, List<bool> EntertainmentList, List<bool> WiFiList, List<bool> ACPowerList, List<string> AirCraftModelList, List<string> AirlineCodeList);
        void CalculatePrice(int adults, int Youths, int Children, string cabin);
    }
}
