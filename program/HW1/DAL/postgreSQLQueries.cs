using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
using HW1.Models;
using Microsoft.Windows.Controls.Primitives;
using Npgsql;

namespace HW1.DAL
{
    internal static class PostgreSQLQueries
    {
        private static string _connectionString =
            @"Server=cpts451termproject.postgres.database.azure.com;Database=project;Port=5432;User Id = admin123@cpts451termproject;Password=Admin$123";

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
        internal static List<Business> GetBusinessesSortedAttributes(City city, State state, String zipcode, List<String> categories, string sortyBy, List<String> attributes, List<String> priceAttributes, User u)
        {
            List<Business> businesses = new List<Business>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();
                String sub1 =
                    $"(select distinct B.businessName,B.City,B.st,B.id,B.reviewRating,B.latitude,B.longitude,B.numCheckins,B.reviewCount,B.stars,B.businessAddress, distance({u.latitude}::double precision, {u.longitude}::double precision,B.latitude, B.longitude) as distance from Business as B, BelongsToCategory as C where B.id = C.businessId and B.zipcode = {zipcode} and B.city = '{city.Name}' and B.st = '{state.Name}'";
                for (int i = 0; i < categories.Count; i++)
                {
                    if (i == 0)
                        sub1 += $" and (C.categoryName = '{categories[i]}'";
                    else
                        sub1 += $" or C.categoryName = '{categories[i]}'";
                    if (i == categories.Count - 1)
                        sub1 += ")";
                }
                sub1 += $"group by B.businessName,B.City,B.st,B.id  Having COUNT(*) = {categories.Count})";
                String sub2 =
                    $"(select distinct B.businessName,B.City,B.st,B.id,B.reviewRating,B.latitude,B.longitude,B.numCheckins,B.reviewCount,B.stars,B.businessAddress,distance({u.latitude}::double precision, {u.longitude}::double precision,B.latitude, B.longitude) as distance from Business as B, Attributes as A where B.id = A.businessId and B.zipcode = {zipcode} and B.city = '{city.Name}' and B.st = '{state.Name}'";
                for (int i = 0; i < attributes.Count; i++)
                {
                    if (i == 0)
                        sub2 += $" and A.attributeValue != 'False' and (A.attributeName = '{attributes[i]}' ";
                    else
                        sub2 += $" or A.attributeName = '{attributes[i]}'";
                    if (i == attributes.Count - 1 && priceAttributes.Count == 0)
                        sub2 += ")";
                }
                for (int i = 0; i < priceAttributes.Count; i++)
                {
                    if (i == 0 && attributes.Count == 0)
                        sub2 += $" and A.attributeName = 'RestaurantsPriceRange2' and (A.attributeValue = '{priceAttributes[i]}' ";
                    else
                        sub2 += $" or (A.attributeValue = '{priceAttributes[i]}' and A.attributeName = 'RestaurantsPriceRange2')";
                    if (i == priceAttributes.Count - 1)
                        sub2 += ")";
                }
                sub2 += $"group by B.businessName,B.City,B.st,B.id  Having COUNT(*) = {attributes.Count + priceAttributes.Count})";
                String sub3 = "";
                if(categories.Count > 0)
                {
                    sub3 = "where query1.id = query2.id";
                }
                cmd.CommandText = $"select distinct query2.businessName,query2.City,query2.st,query2.id,query2.reviewRating,query2.latitude,query2.longitude,query2.numCheckins,query2.reviewCount,query2.stars,query2.businessAddress, query2.distance from {sub1} as query1, {sub2} as query2 {sub3} group by query2.businessName,query2.City,query2.st,query2.id,query2.reviewRating,query2.latitude,query2.longitude,query2.numCheckins,query2.reviewCount,query2.stars,query2.businessAddress, query2.distance order by query2.{sortyBy}";
                if(categories.Count == 0 && (attributes.Count > 0 || priceAttributes.Count > 0))
                {
                    cmd.CommandText = $"{sub2} order by {sortyBy}";
                }
                else if (categories.Count == 0 && attributes.Count == 0 && priceAttributes.Count == 0)
                {
                    cmd.CommandText = $"select distinct businessName,City,st,id,reviewRating,latitude,longitude,numCheckins,reviewCount,stars,businessAddress, distance({u.latitude}::double precision, {u.longitude}::double precision,latitude, longitude) as distance from business where zipcode = {zipcode} and city = '{city.Name}' and st = '{state.Name}' order by {sortyBy}";
                }
                else if (categories.Count > 0 && attributes.Count == 0 && priceAttributes.Count == 0)
                {
                    cmd.CommandText = $"{sub1} order by {sortyBy}";
                }

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Business business = new Business();
                        business.Name = reader.GetString(0);
                        business.CityName = reader.GetString(1);
                        business.StateName = reader.GetString(2);
                        business.id = reader.GetString(3);
                        business.Rating = reader[4] as Double? ?? 0;
                        business.latitude = reader[5] as double? ?? 0;
                        business.longitude = reader[6] as double? ?? 0;
                        business.numCheckins = reader[7] as int? ?? 0;
                        business.reviewCount = reader[8] as int? ?? 0;
                        business.stars = reader[9] as Double? ?? 0;
                        business.Address = reader.GetString(10);
                        business.Distance = Convert.ToString(reader[11] as Double? ?? 0);
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

        internal static List<String> GetBusinessCategory(string id)
        {
            List<String> categories = new List<String>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();
                cmd.CommandText =
                    $"select distinct categoryName from Business as B, BelongsToCategory C where B.id = C.businessId and B.id = '{id}'";
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(reader.GetString(0));
                    }
                }
            }

            return categories;
        }

        internal static List<ListBoxItem> GetAllCategories()
        {
            List<ListBoxItem> categories = new List<ListBoxItem>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();
                cmd.CommandText =
                    $"select distinct categoryName from Business";
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
                    $"select R.userId,R.datecreated,R.stars,R.funny,R.useful,R.cool,R.reviewtext,U.username from Review as R, users as U where businessId = '{business.id}' and U.id = R.userId";
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Review r = new Review();
                        r.UserId = reader.GetString(0);
                        r.DateCreated = reader.GetDate(1).ToString();
                        r.Stars = reader.GetInt32(2);
                        r.Funny = reader.GetInt32(3);
                        r.Useful = reader.GetInt32(4);
                        r.Cool = reader.GetInt32(5);
                        r.Text = reader.GetString(6);
                        r.UserName = reader.GetString(7);
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

        internal static String GetRandomUserId()
        {
            Random random = new Random();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();

                cmd.CommandText = $"Select id from Users order by random() limit 1";
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    return (reader.GetString(0));
                }
            }
        }
        internal static List<User> GetUsers(String userName)
        {
            List<User> users = new List<User>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();
                cmd.CommandText =
                    $"Select distinct id, userName, dateJoined, latitude, longitude, reviewCount, fans, average_stars, funny, useful, cool from Users where userName LIKE '{userName}%' order by id;";
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User user = new User();
                        user.id = reader.GetString(0);
                        user.userName = reader.GetString(1);
                        user.dataJoined = reader.GetDateTime(2);
                        user.latitude = reader[3] as double? ?? 0;
                        user.longitude = reader[4] as double? ?? 0;
                        user.reviewCount = reader[5] as int? ?? 0;
                        user.fans = reader[6] as int? ?? 0;
                        user.average_stars = reader[7] as double? ?? 0;
                        user.funny = reader[8] as int? ?? 0;
                        user.useful = reader[9] as int? ?? 0;
                        user.cool = reader[10] as int? ?? 0;
                        users.Add(user);
                    }
                }
                return users;
            }
        }
        internal static List<User> GetFriends(String userId)
        {
            List<User> friends = new List<User>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();
                cmd.CommandText =
                    $"Select distinct id, userName, dateJoined, latitude, longitude, reviewCount, fans, average_stars, funny, useful, cool from Users, Friendship where friendId = id AND userId = '{userId}' order by id;";
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User user = new User();
                        user.id = reader.GetString(0);
                        user.userName = reader.GetString(1);
                        user.dataJoined = reader.GetDateTime(2);
                        user.latitude = reader[3] as double? ?? 0;
                        user.longitude = reader[4] as double? ?? 0;
                        user.reviewCount = reader[5] as int? ?? 0;
                        user.fans = reader[6] as int? ?? 0;
                        user.average_stars = reader[7] as int? ?? 0;
                        user.funny = reader[8] as int? ?? 0;
                        user.useful = reader[9] as int? ?? 0;
                        user.cool = reader[10] as int? ?? 0;
                        friends.Add(user);
                    }
                }
                return friends;
            }
        }
        internal static Attributes GetBusinessAttributes(string businessId)
        {
            Attributes A = new Attributes();
            A.attributeList = "";
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();
                cmd.CommandText =
                    $"Select attributeName,attributeValue from Attributes where businessId = '{businessId}' order by attributeName";
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        A.attributeList += reader.GetString(0);
                        A.attributeList += "(";
                        A.attributeList += reader.GetString(1);
                        A.attributeList += "); ";
                    }
                }
                return A;
            }
        }
        internal static Hours GetBusinessHours(string businessId)
        {
            Hours H = new Hours();
            H.hoursFormatted = "Closed";
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();
                cmd.CommandText =
                    $"Select dayOfTheWeek,timeOpen,timeClose from Hours where businessId = '{businessId}' and dayOfTheWeek = '{System.DateTime.Now.DayOfWeek.ToString()}'";
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        H.hoursFormatted = "Today (";
                        H.hoursFormatted += reader.GetString(0);
                        H.hoursFormatted += ")\n Opens: ";
                        H.hoursFormatted += reader.GetString(1);
                        H.hoursFormatted += "\n Closes: ";
                        H.hoursFormatted += reader.GetString(2);
                    }
                }
                return H;
            }
        }

        internal static List<Business> GetFavoriteBusinesses(string userId)
        {
            List<Business> bs = new List<Business>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();
                cmd.CommandText =
                    $"select distinct businessName,City,st,id,reviewRating,latitude,longitude,numCheckins,reviewCount,stars,businessAddress,zipcode from business as b, favorites as f where b.id = f.businessId and f.userId = '{userId}';";
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Business business = new Business();
                        business.Name = reader.GetString(0);
                        business.CityName = reader.GetString(1);
                        business.StateName = reader.GetString(2);
                        business.id = reader.GetString(3);
                        business.Rating = reader[4] as Double? ?? 0;
                        business.latitude = reader[5] as double? ?? 0;
                        business.longitude = reader[6] as double? ?? 0;
                        business.numCheckins = reader[7] as int? ?? 0;
                        business.reviewCount = reader[8] as int? ?? 0;
                        business.stars = reader[9] as Double? ?? 0;
                        business.Address = reader.GetString(10);
                        business.Zipcode = reader[11] as int? ?? 0;
                        bs.Add(business);
                    }
                }
                return bs;
            }
        }

        internal static List<FriendsReviewBusinessRow> GetFriendsReviewing(List<User> friends)
        {
            List<FriendsReviewBusinessRow> bs = new List<FriendsReviewBusinessRow>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();
                String orStatements = "";
                foreach (User user in friends)
                {
                    orStatements += $"(r.userId = '{user.id}' and b.id = r.businessId and r.dateCreated=(SELECT MAX(rt.dateCreated) FROM review as rt WHERE rt.userId = '{user.id}')) or";
                }

                orStatements = orStatements.Substring(0, orStatements.Length - 3);
                cmd.CommandText =
                    $"select r.userId, businessName, city, r.reviewText from business as b, review as r where {orStatements};";
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        String userId = reader.GetString(0);
                        User u = friends.Find(x => x.id == userId);

                        FriendsReviewBusinessRow row = new FriendsReviewBusinessRow();
                        row.UserName = u.userName;
                        row.City = reader.GetString(2);
                        row.Text = reader.GetString(3);
                        row.BusinessName = reader.GetString(1);

                        bs.Add(row);
                    }
                }
                return bs;
            }
        }

        internal static void UpdateUserLocation(User user)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();

                cmd.CommandText =
                    $"update Users set longitude = {user.longitude}, latitude = {user.latitude} where id = '{user.id}';";
                cmd.ExecuteNonQuery();
            }
        }

        internal static void RemoveFavoriteBusiness(String businessId, String userId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();

                cmd.CommandText =
                    $"delete from favorites where userId = '{userId}' and businessId = '{businessId}';";
                cmd.ExecuteNonQuery();
            }
        }

        internal static List<Business> FillInAttributes(List<Business> bs)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();
                String orStatements = "";
                foreach (Business b in bs)
                {
                    orStatements += $"(businessId = '{b.id}') or";
                }

                orStatements = orStatements.Substring(0, orStatements.Length - 3);
                cmd.CommandText =
                    $"select businessId, attributeName, attributeValue from Attributes where {orStatements};";
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        String bid = reader.GetString(0);
                        Business b = bs.Find(x => x.id == bid);
                        b.Attributes.Add(reader.GetString(1), reader.GetString(2));
                    }
                }
                return bs;
            }
        }

        internal static void InsertCheckin(string businessId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();
                DateTime dt = DateTime.Now;
                cmd.CommandText =
                    $"select * from checkin where businessId = '{businessId}' and timeofday = '{dt.Hour}:00' and dayoftheweek = '{dt.DayOfWeek}';";
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Close();
                        cmd.CommandText =
                            $"update checkin set checkincount = checkincount + 1 where businessId = '{businessId}' and timeofday = '{dt.Hour}:00' and dayoftheweek = '{dt.DayOfWeek}';";
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        reader.Close();
                        cmd.CommandText =
                            $"insert into checkin (businessId,timeofday,dayOfTheWeek,checkincount) values ('{businessId}', '{dt.Hour}:00','{dt.DayOfWeek}', 1);";
                        cmd.ExecuteNonQuery();
                    }
                }




            }

        }
        internal static void InsertIntoFavorites(string businessId, string userId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();

                cmd.CommandText =
                    $"insert into favorites (UserId, BusinessId) values ('{userId}','{businessId}');";
                cmd.ExecuteNonQuery();
            }
        }

        internal static List<FriendReviewBusinessRow> GetFriendsReviewingBusiness(List<User> friends, string bid)
        {
            List<FriendReviewBusinessRow> bs = new List<FriendReviewBusinessRow>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();
                String orStatements = "";
                foreach (User user in friends)
                {
                    orStatements += $"(r.userId = '{user.id}' and r.userId = u.id and b.id = r.businessId and b.id = '{bid}') or";
                }

                orStatements = orStatements.Substring(0, orStatements.Length - 3);
                cmd.CommandText =
                    $"select u.username, r.dateCreated, r.stars, r.reviewText from business as b, review as r, users as u where {orStatements};";
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        String userId = reader.GetString(0);
                        User u = friends.Find(x => x.id == userId);

                        FriendReviewBusinessRow row = new FriendReviewBusinessRow();
                        row.Username = reader.GetString(0);
                        row.DateCreated = reader.GetDateTime(1);
                        row.Stars = reader[2] as int? ?? 0;
                        row.Review = reader.GetString(3);

                        bs.Add(row);
                    }
                }
                return bs;
            }
        }

        internal static List<Bar> GetBarsForBusiness(string bid)
        {
            List<Bar> bars = new List<Bar>();
            Dictionary<string, Bar> barDict = new Dictionary<string, Bar>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlCommand cmd = connection.CreateCommand();
                cmd.CommandText =
                    $"select dayoftheweek, timeOfDay, checkInCount from Checkin where businessId = '{bid}';";
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        String dayofweek = reader.GetString(0);
                        int checkincount = reader[2] as int? ?? 0;
                        if (barDict.ContainsKey(dayofweek))
                        {
                            barDict[dayofweek].Value += checkincount;
                        }
                        else
                        {
                            barDict.Add(dayofweek, new Bar(dayofweek, checkincount));
                        }
                    }
                }

                foreach (var d in barDict)
                {
                    bars.Add(d.Value);
                }
                return bars;
            }
        }
    }
}
