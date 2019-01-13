using Kaamkaaz.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Kaamkaaz.Services
{
    public static class KaamkaazService
    {
        public static bool Broadcast(BroadcastMessage message)
        {
            return true;
        }
        public static List<string> GetSericeTypes(string contry)
        {
            var list = new List<string>();
            list.Add("Choose a service");
            list.Add("Auto-riksha");
            list.Add("Electrician");
            list.Add("Plumber");
            list.Add("Taxi");
            list.Add("Driver");
            list.Add("Housemaid");
            list.Add("Grocery Store");
            list.Add("Painter");
            list.Add("Caterer");
            list.Add("Mechanic");
            list.Add("Tutor");
            list.Add("Volunteer");
            return list;
        }
    }
}
