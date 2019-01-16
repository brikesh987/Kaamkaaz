using System;
using System.Collections.Generic;
using System.Text;

namespace Kaamkaaz.Models
{
    public class Profile
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public List<string> Services { get; set; } = new List<string>();
    }
}
