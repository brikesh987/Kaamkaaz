using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaamkaazServices.Models
{
    public class User
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> ServicesOffered { get; set; } = new List<string>();
        public bool IsActive { get; set; }
        public string AboutUser { get; set; }
        public Location Location { get; set; }
    }
}
