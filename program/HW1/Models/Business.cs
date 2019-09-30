using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW1.Models
{
    class Business
    {
        public Business()
        {
            Attributes = new Dictionary<string, string>();
        }
        public String id { get; set; }
        public String Name { get; set; }
        public String StateName { get; set; }
        public String CityName { get; set; }
        public String Address { get; set; }
        public int Zipcode { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int numCheckins { get; set; }
        public int reviewCount { get; set; }
        public double stars { get; set; }
        public double Rating { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public String Distance { get; set; }
    }
}
