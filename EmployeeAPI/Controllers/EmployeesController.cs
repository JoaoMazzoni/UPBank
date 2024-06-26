using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeAPI.Data;
using Models;
using Models.DTO;
using Models.Utils;
using Models.CopyClasses;
using System.Data;

namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeAPIContext _context;
        private readonly AddressService _addressService;

        public EmployeesController(EmployeeAPIContext context)
        {
            _context = context;
            _addressService = new();
        }

        [HttpGet] 
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee()
        {
            if (_context.Employee == null)
            {
                return NotFound();
            }
            try
            {
                var employees = await _context.Employee.ToListAsync();
                foreach (var employee in employees)
                {
                    employee.Address = await _addressService.GetAddressByAPI(employee.AddressId);
                }

                return employees;
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("{document}")]
        public async Task<ActionResult<Employee>> GetEmployee(string document)
        {
            if (_context.Employee == null)
            {
                return NotFound();
            }
            try
            {
                var employee = await _context.Employee.FindAsync(document);

                if (employee == null)
                {
                    return NotFound();
                }

                employee.Address = _addressService.GetAddressByAPI(employee.AddressId).Result;

                return employee;
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("get/managers")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeeManagers()
        {
            if (_context.Employee == null)
            {
                return NotFound();
            }
            try
            {
                var employees = await _context.Employee.Where(e => e.Manager == true).ToListAsync();

                if (employees == null)
                    return NotFound("there are no managers");

                foreach (var employee in employees)
                {
                    employee.Address = await _addressService.GetAddressByAPI(employee.AddressId);
                }

                return employees;
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // PUT: api/Employees/5
        [HttpPut("{document}")]
        public async Task<IActionResult> PutEmployee(string document, Employee employee)
        {
            if (document != employee.Document)
            {
                return BadRequest();
            }
            try
            {
                var e = await _context.Employee.FindAsync(document);

                if (e != null && e.Register != employee.Register)
                {
                    return BadRequest("It is not possible to change the registration number");
                }
                _context.Entry(employee).State = EntityState.Modified;
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest("Unable to find an employee with the document inserted");
            }
            catch (Exception ex)
            {
                return Problem();
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(document))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Employees
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(EmployeeDTO employeeDTO)
        {
            if (_context.Employee == null)
            {
                return Problem("Entity set 'EmployeeAPIContext.Employee'  is null.");
            }


            try
            {
                employeeDTO.Document = CPFValidator.FormatCPF(employeeDTO.Document);
                
                CPFValidator.IsValid(employeeDTO.Document);

                var deletedEmployee = await _context.DeletedEmployee.FindAsync(employeeDTO.Document);

                if (deletedEmployee != null)
                    return BadRequest("This employee has a deleted record");

                var employee = BuildEmployee(employeeDTO).Result;

                _context.Employee.Add(employee);

                try
                {
                    await _context.SaveChangesAsync();
                    return CreatedAtAction("GetEmployee", new { document = employee.Document }, employee);
                }
                catch (DbUpdateException)
                {
                    if (EmployeeExists(employee.Document))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem();
            }

            return NoContent();
        }

        // DELETE: api/Employees/5
        [HttpDelete("{document}")]
        public async Task<IActionResult> DeleteEmployee(string document)
        {
            if (_context.Employee == null)
            {
                return NotFound();
            }
            
            document = CPFValidator.FormatCPF(document);
            
            var employee = await _context.Employee.FindAsync(document);

            if (employee == null)
            {
                return NotFound();
            }

            DeletedEmployee deletedEmployee = new(employee);
            try
            {
                _context.Employee.Remove(employee);
                _context.DeletedEmployee.Add(deletedEmployee);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return Problem(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

            return NoContent();
        }

        [HttpPatch("{manager}/aprove/{customer}")]
        public async Task<IActionResult> AproveAccount(string customer, string manager)
        {
            try
            {
                var employee = await _context.Employee.FindAsync(manager);

                if (employee == null)
                {
                    return NotFound("not was possible to find this employee, Document: " + manager);
                }
                if (employee.Manager)
                {
                    var customerApi = new ApiConsumer<IActionResult>("https://localhost:7045/api/Customers/");
                    var customerAccepted = await customerApi.PatchReturnAction(customer);

                    if (customerAccepted != null)
                        return NoContent();
                    else
                        throw new Exception("there was an error communicating between the APIs");
                }
                else
                    return Problem("This employee not is a manager");
            }
            catch (Exception ex)
            {
                return Problem(); ;
            }
        }

        [HttpPost("{manager}/set/account")]
        public async Task<ActionResult<Account>> SetAccountProfile(string manager, AccountDTO accountDTO)
        {
            try
            {
                var employee = await _context.Employee.FindAsync(manager);

                if (employee == null)
                {
                    return BadRequest("not was possible to find this employee, Document: " + manager);
                }
                if (employee.Manager)
                {
                    var accountApi = new ApiConsumer<Account>("https://localhost:7285/api/Accounts");
                    var accountCreated = await accountApi.Post("", accountDTO);

                    return accountCreated;
                }
                else
                    return BadRequest("This employee not is a manager");
            }
            catch (Exception ex)
            {
                return Problem();
            }
        }

        private bool EmployeeExists(string document)
        {
            return (_context.Employee?.Any(e => e.Document == document)).GetValueOrDefault();
        }
        private async Task<Employee> BuildEmployee(EmployeeDTO employeeDTO)
        {
            string addressId = employeeDTO.AddressDTO.ZipCode + employeeDTO.AddressDTO.Number;

            try
            {

                Address address = await _addressService.GetAddressByAPI(addressId);

                if (address == null)
                    address = await _addressService.PostAddress(employeeDTO.AddressDTO);

                Employee employee = new Employee(employeeDTO);
                employee.Address = address;

                return employee;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
