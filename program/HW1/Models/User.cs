using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HW1.DAL;
using Npgsql;

namespace HW1.Models
{
    class User
    {
        public String id { get; set; }
        public String userName { get; set; }
        public DateTime dataJoined { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int reviewCount { get; set; }
        public int fans { get; set; }
        public double average_stars { get; set; }
        public int funny { get; set; }
        public int useful { get; set; }
        public int cool { get; set; }
    }
}
