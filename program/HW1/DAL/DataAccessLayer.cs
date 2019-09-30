using System;
using System.Collections.Generic;
using HW1.Models;


namespace HW1.DAL
{
    /*DataAccessLayer consists of multiple functions whoes primary functionality is to grab information by calling
     *functions that in turn grab information from data tables using sql queries*/
    class DataAccessLayer
    {
        //GetStates()-->gets a list of all states
        public List<State> GetStates()
        {
            List<State> states = PostgreSQLQueries.GetStates();
            return states;
        }

        //GetCities()-->gets a list of all citieswithin a certain state
        public List<City> GetCities(State state)
        {
            try
            {
                List<City> cities = PostgreSQLQueries.GetCitiesByState(state);
                return cities;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //GetZipcodes()-->gets a list of all zipcodes using a given state and city 
        public List<String> GetZipcodes(State state, City city)
        {
            try
            {
                List<String> zipcodes = PostgreSQLQueries.GetZipcodes(city, state);
                return zipcodes;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //GetBusinessCategories()-->gets a list of all Business Categories based of given state, city and zipcode
        public List<ListBoxItem> GetBusinessCategories(State state, City city, String zipcode)
        {
            try
            {
                List<ListBoxItem> categories = PostgreSQLQueries.GetBusinessCategory(zipcode, city, state);
                return categories;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //GetBusinessCategories()-->gets a list of all Business Categories based of a single business
        public List<String> GetBusinessCategories(String id)
        {
            try
            {
                List<String> categories = PostgreSQLQueries.GetBusinessCategory(id);
                return categories;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //GetAllBusCategories()-->gets a list of all possible Business Categories
        //!!!!!not used for now!!!!!
        /*public List<ListBoxItem> GetAllBusCategories()
        {
            try
            {
                List<ListBoxItem> categories = PostgreSQLQueries.GetAllCategories();
                return categories;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }*/

        //GetBusinesses()-->gets a list of all Businesses that are withing the 3 given categories (State, City, Zipcode)
        public List<Business> GetBusinesses(State state, City city, String zipcode, string sortby, User u)
        {
            sortby = sortby.Replace(" ", "");
            try
            {
                List<Business> businesses = PostgreSQLQueries.GetBusinessesSortedAttributes(city, state, zipcode, new List<string>(), sortby, new List<string>(), new List<string>(),   u);
                return businesses;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //GetBusinesses()-->gets a list of all Businesses that are withing the 3 given categories (State, City, Zipcode) and
        //are within a selected categorie
        public List<Business> GetBusinessesSorted(State state, City city, List<String> categories, String zipcode, string sortby, User u)
        {
            sortby = sortby.Replace(" ", string.Empty);
            try
            {
                List<Business> businesses = PostgreSQLQueries.GetBusinessesSortedAttributes(city, state, zipcode, categories, sortby, new List<string>(), new List<string>(),  u);
                return businesses;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public List<Business> GetBusinessesSortedAttributes(State state, City city, List<String> categories, String zipcode, string sortby, List<String> attributes, List<String> priceAttributes, User u)
        {
            sortby = sortby.Replace(" ", string.Empty);
            try
            {
                List<Business> businesses = PostgreSQLQueries.GetBusinessesSortedAttributes(city, state, zipcode, categories, sortby, attributes, priceAttributes, u);
                return businesses;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //GetReviews()-->gets all reviews for a business based on its name
        public List<Review> GetReviews(Business business)
        {
            try
            {
                List<Review> reviews = PostgreSQLQueries.GetReviews(business);
                return reviews;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //SubmitReview()-->submits a review (inserts new entry into tablem of review text and stars) for a Business by a user
        public void SubmitReview(String review, Business business, String userId, String stars)
        {
            try
            {
                PostgreSQLQueries.SubmitReview(review, business, userId, stars);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
        }

        //getRandomUser()-->gets a random user id
        //was used for earlier stages
        public String getRandomUser()
        {
            try
            {
                return (PostgreSQLQueries.GetRandomUserId());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //GetUsers()--> gets Users based on the start fo a string
        public List<User> GetUsers(String userName)
        {
            try
            {
                List<User> users = PostgreSQLQueries.GetUsers(userName);
                return users;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //GetFriends()-->gets all friends that a certain  user ID has
        public List<User> GetFriends(String userId)
        {
            try
            {
                List<User> users = PostgreSQLQueries.GetFriends(userId);
                return users;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //GetBusinessAttributes()-->gets attributes that a business has based of its id
        public Attributes GetBusinessAttributes(string businessId)
        {
            try
            {
                Attributes A = PostgreSQLQueries.GetBusinessAttributes(businessId);
                return A;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //GetBusinessHours()-->gets Bussiness hours for a given business ID
        public Hours GetBusinessHours(string businessId)
        {
            try
            {
                Hours H = PostgreSQLQueries.GetBusinessHours(businessId);
                return H;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //GetFavoriteBusinesses()-->gets users favourited Businesses
        public List<Business> GetFavoriteBusinesses(string userId)
        {
            try
            {
                List<Business> b = PostgreSQLQueries.GetFavoriteBusinesses(userId);
                return b;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //GetFriendsReviewing()-->gets all friends of a user that have reviewed within the last day
        public List<FriendsReviewBusinessRow> GetFriendsReviewing(List<User> friends)
        {
            if (friends == null || friends.Count == 0)
                return new List<FriendsReviewBusinessRow>();
            try
            {
                List<FriendsReviewBusinessRow> b = PostgreSQLQueries.GetFriendsReviewing(friends);
                return b;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //UpdateUserLocation()-->updates the user location after a new latitude and longitude has been entered
        public void UpdateUserLocation(User user)
        {
            try
            {
                PostgreSQLQueries.UpdateUserLocation(user);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //RemoveFavoriteBusiness()-->removes a certain Business from a users favourite list
        public void RemoveFavoriteBusiness(Business selectedFavoriteBusiness, User user)
        {
            try
            {
                PostgreSQLQueries.RemoveFavoriteBusiness(selectedFavoriteBusiness.id, user.id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //GetBusinessesWithAttributes()-->gets all bussinesses at a location using: state, city, zipcode- and their attributes
        public List<Business> GetBusinessesWithAttributesSorted(State state, City city, String zipcode, string sortString, User u)
        {
            sortString = sortString.Replace(" ", string.Empty);
            try
            {
                List<Business> businesses = PostgreSQLQueries.GetBusinessesSortedAttributes(city, state, zipcode,new List<String>(), sortString,new List<string>(),new List<string>(),   u);
                return businesses;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //InsertCheckin()-->Inserts a checking  from a user into business table
        public void InsertCheckin(Business b)
        {
            try
            {
                PostgreSQLQueries.InsertCheckin(b.id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //AddToFavorites()-->adds a certain business to favourites of the user, it later can be removwed if needed
        public void AddToFavorites(Business b, User u)
        {
            try
            {
                PostgreSQLQueries.InsertIntoFavorites(b.id, u.id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<FriendReviewBusinessRow> GetFriendsReviewingBusiness(Business business, List<User> friends)
        {
            if (friends == null || friends.Count == 0)
                return new List<FriendReviewBusinessRow>();
            try
            {
                List<FriendReviewBusinessRow> b = PostgreSQLQueries.GetFriendsReviewingBusiness(friends, business.id);
                return b;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public List<Bar> GetBarsForBusiness(string id)
        {
            try
            {
                List<Bar> b = PostgreSQLQueries.GetBarsForBusiness(id);
                return b;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
