﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace DatabaseConnection
{
    class Seeding
    {
        static void Main() 
        {
            using (var ctx = new Context())
            {
                ctx.RemoveRange(ctx.Sales);
                ctx.RemoveRange(ctx.Movies);
                ctx.RemoveRange(ctx.Customers);

                ctx.AddRange(new List<Customer> {

                    new Customer { FirstName = "Björn",    Password =  "Björn" , LastName   = "StrömBerg" },
                    new Customer { FirstName = "Robin",    Password =  "Robin" , LastName   = "Havberg"   },
                    new Customer { FirstName = "Amanda",   Password =  "Amanda", LastName   = "Åberg"     },

                    // Added in migration - MoreCustomers
                    new Customer { FirstName = "Erik",     Password =  "Erik"      ,  LastName   =  "StrömBerg" },
                    new Customer { FirstName = "Johannes", Password =  "Johannes"  ,  LastName   =  "Åberg"     },
                    new Customer { FirstName = "Martin",   Password =  "Martin"    ,  LastName   =  "Havberg"   },
                    new Customer { FirstName = "Ida",      Password =  "Ida"       ,  LastName   =  "Åberg"     },
                    new Customer { FirstName = "Eva",      Password =  "Eva"       ,  LastName   =  "StrömBerg" }

                });

                // Här laddas data in från SeedData foldern för att fylla ut Movies tabellen
                var movies = new List<Movie>();
                var lines = File.ReadAllLines(@"..\..\..\SeedData\MovieGenre.csv");
                for (int i = 1; i < 200; i++)
                {
                    // imdbId,Imdb Link,Title,IMDB Score,Genre,Poster
                    var cells = lines[i].Split(',');

                    var url = cells[5].Trim('"');

                    // Hoppa över alla icke-fungerande url:er
                    try{ var test = new Uri(url); }
                    catch (Exception) { continue; }

                    movies.Add(new Movie { Title = cells[2], ImageURL = url });
                }
                ctx.AddRange(movies);

                ctx.SaveChanges();
            }
        }
    }
}
