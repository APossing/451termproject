using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HW1.DAL;
using Npgsql;

namespace HW1.Models
{
    class State : DatabaseObject<State>
    {
        public string Name { get; set; }

        public State(){}

        public State(string name)
        {
            Name = name;
        }

        public override int ID { get; set; }
        public override string TableName => "States";
        public override string[] SelectArray => new[] {"ID", "Name"};
        public override string[] InsertArray => new[] {"ID", "Name"};
        public override object[] InsertArgs => new object[] {ID, Name};
        internal override State Serialize(NpgsqlDataReader reader)
        {
            State output = new State();
            output.ID = reader.GetInt32(0);
            output.Name = reader.GetString(1);
            return output;
        }
    }
}
