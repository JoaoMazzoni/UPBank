using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class CustomerDTO : PersonDTO
    {
        public bool Restriction { get; set; }
    }
}
