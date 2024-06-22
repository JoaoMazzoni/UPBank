using Models.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [NotMapped]
        public Address Address { get; set; }
        public string AddressId { get; set; }   

        public Person() 
        { 
        }

        public Person(PersonDTO personDTO)
        {
            Name = personDTO.Name;
            Document = personDTO.Document;
            BirthDate = personDTO.BirthDate;
            Gender = personDTO.Gender;
            Salary = personDTO.Salary;
            Email = personDTO.Email;
            Phone = personDTO.Phone;
            AddressId = personDTO.AddressDTO.ZipCode + personDTO.AddressDTO.Number;
        }


    }
}
