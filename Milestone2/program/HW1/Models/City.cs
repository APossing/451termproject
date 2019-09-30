using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HW1.DAL;
using Npgsql;

namespace HW1.Models
{
    class City : DatabaseObject<City>
    {
        public String Name { get; set; }

        public City()
        {
        }

        public City(String name)
        {
            Name = name;
        }

        public override int ID { get; set; }
        public override string TableName => "Cities";
        public override string[] SelectArray => new[] {"ID", "Name"};
        public override string[] InsertArray => new[] {"ID", "Name"};
        public override object[] InsertArgs => new object[] {ID, Name};
        internal override City Serialize(NpgsqlDataReader reader)
        {
            City output = new City();
            output.ID = reader.GetInt32(0);
            output.Name = reader.GetString(1);
            return output;
        }
    }
}
