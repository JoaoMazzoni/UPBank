using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class PersonDTO
    {
        public string Name { get; set; }
        [Key, MaxLength(14), MinLength(14)]
        public string Document { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public double Salary { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public AddressDTO AddressDTO { get; set; }  
    }

}
