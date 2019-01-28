using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaamkaazServices.Models
{
    public class ServiceProvider
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string AboutUser { get; set; }
        public string Service { get; set; }
        public Location Location { get; set; }
    }
    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int UserId { get; set; }
    }
}
