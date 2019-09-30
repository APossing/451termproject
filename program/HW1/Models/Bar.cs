using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW1.Models
{
    class Bar
    {
        public Bar(string label, int value)
        {
            Label = label;
            Value = value;
        }
        public string Label { get; set; }
        public int Value { get; set; }
    }
}
