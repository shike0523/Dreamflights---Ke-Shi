using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DreamFlights.Models
{
    public class City
    {
        public int CityID { get; set; }
        [Required]
        [StringLength(25)]
        public string Name { get; set; }
        //[Required]
        [StringLength(60)]
        public string AirportName { get; set; }
        [Range(1, 9999, ErrorMessage = "The formal is incorrect")]
        public int PostCode { get; set; }
        [Required]
        public StateOrTerritory StateOrTerritory { get; set; }
        public string State { get; set; }
        //{
        //    get
        //    {
        //        return StateOrTerritory.ToString();
        //    }
        //    set
        //    {
        //        State = "v";
        //    }
        //}

    }

    public enum StateOrTerritory
    {
        NSW, QLD, SA, TAS, VIC, WA, ACT, JBT, NT
    }
}
