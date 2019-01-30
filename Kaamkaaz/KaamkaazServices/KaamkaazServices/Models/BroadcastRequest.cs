using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaamkaazServices.Models
{
    public class BroadcastRequest
    {
        public Location CurrentLocation { get; set; } = new Location();
        public string ServiceRequested { get; set; } = string.Empty;
        public string MessageBody { get; set; } = string.Empty;
        public int UserId { get; set; }
        public bool IsValid()
        {
            if (CurrentLocation == null || CurrentLocation.Latitude == 0 || CurrentLocation.Longitude == 0 || 
                string.IsNullOrWhiteSpace(CurrentLocation.City) || string.IsNullOrWhiteSpace(CurrentLocation.Country)){
                return false;
            }
            if (string.IsNullOrWhiteSpace(ServiceRequested))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(MessageBody))
            {
                return false;
            }
            if (UserId <= 0)
            {
                return false;
            }
            //ToDO: Hack location needs userId. May be the userId can be removed from BroadCastRequest
            CurrentLocation.UserId = UserId;
            return true;
        }
    }
}
