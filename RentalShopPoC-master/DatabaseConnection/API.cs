using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace DatabaseConnection
{
    public static class API
    {
        static Context ctx;

        static API()
        {
            ctx = new Context();
        }
        public static List<Movie> GetMovieSlice(int a, int b)
        {
            return ctx.Movies.OrderBy(m => m.Title).Skip(a).Take(b).ToList();
        }
        public static Customer GetCustomerByName(string name)
        {
            return ctx.Customers.FirstOrDefault(c => c.Name.ToLower() == name.ToLower());
        }
        public static bool RegisterSale(Customer customer, Movie movie)
        {
            try
            {
                ctx.Add(new Rental() { Date = DateTime.Now, Customer = customer, Movie = movie });
                return ctx.SaveChanges() == 1;
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
