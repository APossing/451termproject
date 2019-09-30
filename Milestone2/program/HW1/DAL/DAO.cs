using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace HW1.DAL
{
    internal abstract class DatabaseObject<T> : DatabaseTable where T : DatabaseObject<T>, new()
    {
        internal abstract T Serialize(NpgsqlDataReader reader);


        public static List<T> Get()
        {
            DatabaseObject<T> handlerObject = new T();

            string query = "SELECT " + handlerObject.SelectString + " FROM " + handlerObject.TableName;
            /*
            NpgsqlConnection connection = ConnectionFactory.GetConnection();
            NpgsqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = query;
            NpgsqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) output.Add(handlerObject.Serialize(reader));*/
            List<T> output = new List<T>();
            return output;

        }
    }
}
