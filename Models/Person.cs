using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public abstract class Person
    { 
        [Key]
        public string Document { get; set; }
        public string Name { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public double Salary { get; set;}
        public string Email { get; set; }
        public string Phone { get; set; }
        public Address Address { get; set; }
    }
}
