using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DreamFlights.Controllers.HomeController;

namespace DreamFlights.ViewModels
{
    public class CandidateTripVM
    {
        //List<candidateTrip> routeList = new List<candidateTrip>();
        public List<int> RouteList { get; set; }
        public List<string> stopList { get; set; }
        public DateTime DepartTime { get; set; }
        public DateTime ArrivetTime { get; set; }
        public int TotalPrice { get; set; }
        public List<string> AirlineCodeList { get; set; }
        public List<bool> EntertainmentList { get; set; }
        public List<bool> ACPowerList { get; set; }
        public List<bool> WiFiList { get; set; }
        public List<string> AirCraftModelList { get; set; }
    }
}
