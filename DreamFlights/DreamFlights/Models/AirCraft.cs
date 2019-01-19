using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamFlights.Models
{
    public class AirCraft
    {
        public int AirCraftID { get; set; }
        public string AirCraftModel { get; set; }
        public int EconomyCap { get; set; }
        public int BusinessCap { get; set; }
        public int PremEconomyCap { get; set; }
        public int FirstCap { get; set; }
        public bool Entertainment { get; set; }
        public bool ACPower { get; set; }
        public bool WiFi { get; set; }
    }
}
