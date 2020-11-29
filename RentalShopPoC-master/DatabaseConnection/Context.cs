using Microsoft.EntityFrameworkCore;
using System;

namespace DatabaseConnection
{
    public class Context : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Rental> Sales { get; set; }

        private DbContextOptions _options;

        public Context()
        {

        }
        public Context(DbContextOptions<Context> options) //for testing with InMemDb
            : base(options)
        {
            _options = options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if(_options == null) //If not inMemDb.
                optionsBuilder
                    //.LogTo(s => System.Diagnostics.Debug.WriteLine(s))
                    .UseLazyLoadingProxies()
                    .UseSqlServer(
                    @"server=localhost\SQLEXPRESS;" +
                    @"database=SaleDatabase;" +
                    @"trusted_connection=true;" +
                    @"MultipleActiveResultSets=True"
                    );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
