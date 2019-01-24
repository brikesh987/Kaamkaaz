using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaamkaazServices.Models
{
    public class BroadcastRequest
    {
        public Location CurrentLocation { get; set; }
        public string ServiceRequested { get; set; }
        public string MessageBody { get; set; }
    }
}
