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
    }
}
