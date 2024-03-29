﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaamkaazServices.Models
{
    public class ServiceProvidersRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Service { get; set; }
        public string City { get; set; }
        public int UserId { get; set; }
        public bool IsValid()
        {
            if (Latitude != 0 && Longitude != 0 && !string.IsNullOrWhiteSpace(City))
            {
                return true;
            }
            return false;
        }
        public string GetCacheKey()
        {
            var builder = new StringBuilder();
            builder.Append(City);
            builder.Append("_");
            builder.Append(Latitude);
            builder.Append("_");
            builder.Append(Longitude);
            return builder.ToString();
        }
    }
}
