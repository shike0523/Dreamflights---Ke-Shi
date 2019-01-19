using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DreamFlights.Models
{
    public class Route
    {
        public int RouteID { get; set; }
        [Required]
        public int FromCityID { get; set; }
        [Required]
        public int ToCityID { get; set; }
        [Required]
        public int BasicFare { get; set; }
        public City FromCity { get; set; }
        public City ToCity { get; set; }
        [DataType(DataType.Time)]
        [Range(typeof(TimeSpan), "00:00", "12:00")]
        [DisplayFormat(DataFormatString = @"{0:hh\:mm}")]
        public TimeSpan AveDuration { get; set; }        
    }
}
