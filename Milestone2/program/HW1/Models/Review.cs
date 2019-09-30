using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW1.Models
{
    class Review
    {
        public String UserId { get; set; }
        public String DateCreated { get; set; }
        public int Stars { get; set; }
        public int Funny { get; set; }
        public int Useful { get; set; }
        public int Cool { get; set; }
        public String Text { get; set; }
    }
}
