using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW1.Models
{
    class FriendReviewBusinessRow
    {
        public string Username { get; set; }
        public DateTime DateCreated { get; set; }
        public int Stars { get; set; }
        public string Review { get; set; }
    }
}
