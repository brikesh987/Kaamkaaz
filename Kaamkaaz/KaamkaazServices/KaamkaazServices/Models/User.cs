using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaamkaazServices.Models
{
    public class User
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public List<string> ProfileData { get; set; } = new List<string>();
        public bool IsActive { get; set; } = false;
        public string AboutUser { get; set; } = string.Empty;
        public Location Location { get; set; } = new Location();
        public string UserId { get; set; } = string.Empty;
        public int Id { get; set; }
    }
}
