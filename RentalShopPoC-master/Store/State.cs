using System;
using System.Collections.Generic;
using System.Text;
using DatabaseConnection;
using Microsoft.EntityFrameworkCore;

namespace Store
{
    // Håller koll på den data som programmet måste komma ihåg mellan vyer/fönster förändringar
    static class State
    {
        public static Customer User { get; set; }
        public static List<Movie> Movies { get; set; }
        public static Movie Pick { get; set; }
    }
}
