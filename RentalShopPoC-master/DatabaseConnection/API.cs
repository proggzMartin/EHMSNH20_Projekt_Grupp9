using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace DatabaseConnection
{
    public static class API
    {
        static Context context;

        static API()
        {
            context = new Context();
        }
        public static List<Movie> GetMovieSlice(int a, int b)
        {
            return context.Movies.OrderBy(m => m.Title).Skip(a).Take(b).ToList();
        }

        public static int GetNumOfMovies()
        {
            return context.Movies.Count();
        }

        public static Customer GetCustomerByName(string name)
        {
            return context.Customers.FirstOrDefault(c => c.UserEmail.ToLower() == name.ToLower());
        }

        public static bool RegisterSale(Customer customer, Movie movie)
        {
            try
            {
                context.Add(new Rental() { Date = DateTime.Now, Customer = customer, Movie = movie });
                return context.SaveChanges() == 1;
            }
            catch(DbUpdateException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine(e.InnerException.Message);
                return false;
            }
        }

        public static bool IsRentedByCustomer(Customer activeCustomer, int movieId)
        {
            var m = context.Movies.FirstOrDefault(x => x.Id.Equals(movieId)) ?? throw new Exception("Movie doesn't exist.");

            var x = m.Sales.Any(x => x.Customer.UserEmail.Equals(activeCustomer.UserEmail));
            return x;
        }

        public static void RentMovie(Customer activeCustomer, int movieId)
        {
            var customer = context.Customers
                            .FirstOrDefault(x => x.UserEmail.Equals(activeCustomer.UserEmail)) //select customer on primary key.
                                ?? throw new Exception("Customer doesn't exist.");

            customer.Sales?.Add(new Rental()
            {
                MovieId = movieId
            });
            context.SaveChanges();
        }

    }
}
