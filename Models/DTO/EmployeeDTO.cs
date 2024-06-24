using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class EmployeeDTO : PersonDTO
    {
        public bool Manager { get; set; }
        public int Register { get; set; }
    }
}
