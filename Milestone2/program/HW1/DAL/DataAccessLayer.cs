using System;
using System.Collections.Generic;
using HW1.Models;


namespace HW1.DAL
{
    class DataAccessLayer
    {
        public List<State> GetStates()
        {
                List<State> states = PostgreSQLQueries.GetStates();
                return states;
        }

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
        public List<Business> GetBusinesses(State state, City city, String zipcode)
        {
            try
            {
                List<Business> businesses = PostgreSQLQueries.GetBusinesses(city, state, zipcode);
                return businesses;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<Business> GetBusinesses(State state, City city, List<String> categories, String zipcode)
        {
            try
            {
                List<Business> businesses = PostgreSQLQueries.GetBusinesses(city, state, zipcode, categories);
                return businesses;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

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

        public void SubmitReview(String review, Business business, String userId, String stars)
        {
            try
            {
                PostgreSQLQueries.SubmitReview(review,business,userId,stars);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
            }
        }
    }
}
