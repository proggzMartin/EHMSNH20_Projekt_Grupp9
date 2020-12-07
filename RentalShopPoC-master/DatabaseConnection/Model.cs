using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DatabaseConnection
{
    public class Customer
    {
        [Key]
//<<<<<<< HEAD
//        [Required]
//        public string UserEmail { get; set; }
//        [Required]
//        public string Password { get; set; }

//        public string FirstName { get; set; }
//=======
        public int Id { get; set; }      
        [Required]
        public string Name { get; set; }
        [Required] 
        public string Password { get; set; }
        public string LastName { get; set; }
        public virtual List<Rental> Sales { get; set; }
    }
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageURL { get; set; }
        public virtual List<Rental> Sales { get; set; }
    }
    public class Rental
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public virtual Customer Customer { get; set; }
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; }
    }




}
