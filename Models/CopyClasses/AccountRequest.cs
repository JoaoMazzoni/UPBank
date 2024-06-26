using Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CopyClasses
{
    public class AccountRequest : Person
    {
        public bool Restriction { get; set; }

        public AccountRequest()
        {
        }

        public AccountRequest(AccountRequestDTO accountRequestDTO) : base(accountRequestDTO)
        {
            Restriction = accountRequestDTO.Restriction;
        
        }


        public Customer MoveAccountRequest(AccountRequest accountRequest)
        {
            var customer = new Customer
            {
                Document = accountRequest.Document,
                Name = accountRequest.Name,
                BirthDate = accountRequest.BirthDate,
                Gender = accountRequest.Gender,
                Salary = accountRequest.Salary,
                Email = accountRequest.Email,
                Phone = accountRequest.Phone,
                Address = accountRequest.Address,
                AddressId = accountRequest.AddressId
            };

            return customer;
        }

        public AccountRequest ToAccountRequest(RemovedCustomer removed)
        {
            var accountRequest = new AccountRequest
            {
                Document = removed.Document,
                Name = removed.Name,
                BirthDate = removed.BirthDate,
                Gender = removed.Gender,
                Salary = removed.Salary,
                Email = removed.Email,
                Phone = removed.Phone,
                Address = removed.Address,
                AddressId = removed.AddressId
            };

            return accountRequest;
        }

        public AccountRequest EmployeeToCustomer(Employee employee)
        {
            var customer = new AccountRequest
            {
                Document = employee.Document,
                Name = employee.Name,
                BirthDate = employee.BirthDate,
                Gender = employee.Gender,
                Salary = employee.Salary,
                Email = employee.Email,
                Phone = employee.Phone,
                Address = employee.Address,
                AddressId = employee.AddressId
            };
            return customer;
        }

    }
}
