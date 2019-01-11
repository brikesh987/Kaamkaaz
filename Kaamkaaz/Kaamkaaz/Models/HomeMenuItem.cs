using System;
using System.Collections.Generic;
using System.Text;

namespace Kaamkaaz.Models
{
    public enum MenuItemType
    {
        Find,
        About
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
