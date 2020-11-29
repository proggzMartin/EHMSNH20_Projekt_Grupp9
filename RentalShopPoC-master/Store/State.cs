using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseConnection;
using Microsoft.EntityFrameworkCore;

namespace Store
{
    // Håller koll på den data som programmet måste komma ihåg mellan vyer/fönster förändringar
    static class State
    {
        private static Context context = new Context();


        public static Customer User { get; set; } = context.Customers.FirstOrDefault(x => x.UserEmail.Equals("Björn"));
        public static List<Movie> Movies { get; set; }
        public static Movie Pick { get; set; }
    }
}
