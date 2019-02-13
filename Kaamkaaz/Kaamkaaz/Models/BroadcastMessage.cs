using System;
using System.Collections.Generic;
using System.Text;

namespace Kaamkaaz.Models
{
    public class BroadcastMessage
    {
        public string ServiceRequested { get; set; }
        public string MessageBody { get; set; }
        public int UserId { get; set; }
        public Location CurrentLocation { get; set; } = new Location();
    }
   public class Location
    {
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the Country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the Latitude
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the Longitude
        /// </summary>
        public double Longitude { get; set; }
    }
}
