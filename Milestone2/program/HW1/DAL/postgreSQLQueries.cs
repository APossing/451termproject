using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
using HW1.Models;
using Npgsql;

namespace HW1.DAL
{
    internal static class PostgreSQLQueries
    {
        private static string _connectionString =
            @"Server=451termproject.postgres.database.azure.com;Database=project;Port=5432;User Id = admin123@451termproject;Password=Admin$123";

        internal static List<City> GetCitiesByState(State state)
        {
            List<City> cities = new List<City>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = $"Select distinct city from Business where st = '{state.Name}' order by city;";
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        City city = new City();
                        city.Name = reader.GetString(0);
                        cities.Add(city);
                    }
                }
            }

            return cities;
        }

        internal static List<State> GetStates()
        {
            NpgsqlConnection connection;
            List<State> states = new List<State>();
            using (connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    return states;
                }

                NpgsqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = "Select distinct st from business order by st;";

                NpgsqlDataReader reader;
                try
                {
                    reader = cmd.ExecuteReader();
                }
                catch
                {
                    return states;
                }

                while (reader.Read())
                {
                    State state = new State();
                    state.Name = reader.GetString(0);
                    states.Add(state);
                }
            }

            return states;
        }

        internal static List<String> GetZipcodes(City city, State state)
        {
            List<String> zipcodes = new List<String>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();
                cmd.CommandText =
                    $"Select distinct zipcode from Business where city = '{city.Name}' and st = '{state.Name}' order by zipcode;";
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        String zipcode = reader.GetInt32(0).ToString();
                        zipcodes.Add(zipcode);
                    }
                }

                return zipcodes;
            }
        }
        internal static List<Business> GetBusinesses(City city, State state, String zipcode)
        {
            List<Business> businesses = new List<Business>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();
                cmd.CommandText =
                    $"Select distinct businessName,City,st,id,reviewRating from Business where city = '{city.Name}' and st = '{state.Name}' and zipcode = {zipcode} order by businessName;";
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Business business = new Business();
                        business.Name = reader.GetString(0);
                        business.CityName = reader.GetString(1);
                        business.StateName = reader.GetString(2);
                        business.id = reader.GetString(3);
                        business.Rating = reader.GetDouble(4);
                        businesses.Add(business);
                    }
                }

                return businesses;
            }
        }

        internal static List<Business> GetBusinesses(City city, State state, String zipcode, List<String> categories)
        {
            List<Business> businesses = new List<Business>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();
                cmd.CommandText =
                    $"select distinct businessName,City,st,id,count(*),reviewRating from Business as B, BelongsToCategory C where B.id = C.businessId and B.zipcode = {zipcode} and B.city = '{city.Name}' and B.st = '{state.Name}'";
                for (int i = 0; i < categories.Count;i++)
                {
                    if (i == 0)
                        cmd.CommandText += $" and (C.categoryName = '{categories[i]}'";
                    else
                        cmd.CommandText += $" or C.categoryName = '{categories[i]}'";
                    if (i == categories.Count-1)
                        cmd.CommandText += ")";
                }

                cmd.CommandText += "group by businessName,City,st,id;";
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Business business = new Business();
                        business.Name = reader.GetString(0);
                        business.CityName = reader.GetString(1);
                        business.StateName = reader.GetString(2);
                        business.id = reader.GetString(3);
                        business.Rating = reader.GetDouble(5);
                        int count = reader.GetInt32(4);
                        if (count >= categories.Count)
                            businesses.Add(business);
                    }
                }

                return businesses;
            }
        }

        internal static List<ListBoxItem> GetBusinessCategory(string zipcode, City city, State state)
        {
            List<ListBoxItem> categories = new List<ListBoxItem>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();
                cmd.CommandText =
                    $"select distinct categoryName from Business as B, BelongsToCategory C where B.id = C.businessId and B.zipcode = {zipcode} and B.city = '{city.Name}' and B.st = '{state.Name}'";
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ListBoxItem temp = new ListBoxItem();
                        String str = reader.GetString(0);
                        temp.Item = str;
                        categories.Add(temp);
                    }
                }
            }

            return categories;
        }

        internal static List<Review> GetReviews(Business business)
        {
            List<Review> reviews = new List<Review>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();
                cmd.CommandText =
                    $"select * from Review where businessId = '{business.id}'";
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Review r = new Review();
                        r.UserId = reader.GetString(0);
                        r.DateCreated = reader.GetDate(3).ToString();
                        r.Stars = reader.GetInt32(4);
                        r.Funny = reader.GetInt32(5);
                        r.Useful = reader.GetInt32(6);
                        r.Cool = reader.GetInt32(7);
                        r.Text = reader.GetString(8);
                        reviews.Add(r);
                    }
                }
            }

            return reviews;
        }

        internal static void SubmitReview(String review, Business business, String userId, String stars)
        {
            Random random = new Random();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();

                cmd.CommandText =
                    $"insert into review(businessId,userId,reviewId,datecreated,stars,funny,useful,cool,reviewtext) values ('{business.id}', '{userId}', '{random.Next()}','{DateTime.Now.ToString("yyyy-MM-dd")}', {stars}, 0, 0,0,'{review}'); ";
                cmd.ExecuteNonQuery();
            }
        }
    }
}
