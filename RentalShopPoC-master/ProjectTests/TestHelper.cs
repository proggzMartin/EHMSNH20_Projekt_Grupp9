using DatabaseConnection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTests
{
    public static class TestHelper
    {
        public static void CreateInMemDb()
        {
            var options = new DbContextOptionsBuilder<Context>()
                               .UseInMemoryDatabase(databaseName: "MemDb")
                               .Options;

            //return new Context(options);
            using (var context = new Context(options))
            {
                var customer = new Customer
                {
                    FirstName = "Elizabeth",
                    LastName = "Lincoln",
                };

                context.Customers.Add(customer);
                context.SaveChanges();

            }

        }
    }
}
