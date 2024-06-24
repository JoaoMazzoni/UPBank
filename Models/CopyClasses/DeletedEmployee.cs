using Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CopyClasses
{
    public class DeletedEmployee : Person
    {
        public bool Manager { get; set; }
        public int Register { get; set; }
        public DeletedEmployee() { }
        public DeletedEmployee(EmployeeDTO dto) : base(dto)
        {
            Manager = dto.Manager;
        }
        public DeletedEmployee(Employee employee)
        {
            Name = employee.Name;
            Document = employee.Document;
            BirthDate = employee.BirthDate;
            Gender = employee.Gender;
            Salary = employee.Salary;
            Email = employee.Email;
            Phone = employee.Phone;
            AddressId = employee.AddressId;
            Address = employee.Address;

            Manager =employee.Manager;
            Register = employee.Register;
        }
    }
}
