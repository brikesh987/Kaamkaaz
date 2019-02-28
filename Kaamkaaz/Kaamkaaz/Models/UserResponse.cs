using System;
using System.Collections.Generic;
using System.Text;

namespace Kaamkaaz.Models
{
    internal class UserResponse
    {
        public string UserId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
