using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaamkaazServices.Models
{
    public class ServiceProvidersRequest
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Service { get; set; }
    }
}
