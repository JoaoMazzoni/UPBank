using Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Employee : Person
    {
        public bool Manager { get; set; }
        public int Register { get; set; }

        public Employee() { }
        public Employee(EmployeeDTO dto) : base(dto)
        {
            Manager = dto.Manager;
        }
    }
}
