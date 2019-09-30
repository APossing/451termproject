using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW1.DAL
{
    public abstract class DatabaseTable
    {
        public abstract int ID { get; set; }
            public abstract string TableName { get; }
            public abstract string[] SelectArray { get; }
            public abstract string[] InsertArray { get; }
            public abstract object[] InsertArgs { get; }

        internal string SelectString
        {
            get
            {
                string output = "";
                for (int i = 0; i < SelectArray.Length - 1; i++)
                {
                    output += SelectArray[i] + ",";

                }
                output += SelectArray.Last();
                return output;
            }
        }

        internal string InsertString
        {
            get
            {
                string output = "";
                for (int i = 0; i < InsertArray.Length - 1; i++)
                {
                    output += InsertArray[i] + ",";
                }
                output += InsertArray.Last();
                return output;
            }
        }
    }
}
