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

        // GET: api/Employees
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

        // GET: api/Employees/5
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
                employee.Address = _addressService.GetAddressByAPI(employee.AddressId).Result;

                if (employee == null)
                {
                    return NotFound();
                }

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

/*        [HttpGet("get/customers_requests")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomerResquests()
        {
            try
            {
                ApiConsumer<List<Customer>> apiCostumer = new ApiConsumer<List<Customer>>("https://localhost:7045/api/Customers/");

                var costumers = await apiCostumer.Get("byRequest/true", true);

                return costumers;
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }*/

        // PUT: api/Employees/5
        [HttpPut("{document}")]
        public async Task<IActionResult> PutEmployee(string document, Employee employee)
        {
            if (document != employee.Document)
            {
                return BadRequest();
            }

            var e = await _context.Employee.FindAsync(document);

            if (e != null && e.Register != employee.Register)
            {
                return BadRequest("It is not possible to change the registration number");
            }
            _context.Entry(employee).State = EntityState.Modified;

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

            var employee = BuildEmployee(employeeDTO).Result;

            _context.Employee.Add(employee);
            try
            {
                await _context.SaveChangesAsync();
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

            return CreatedAtAction("GetEmployee", new { document = employee.Document }, employee);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{document}")]
        public async Task<IActionResult> DeleteEmployee(string document)
        {
            if (_context.Employee == null)
            {
                return NotFound();
            }
            var employee = await _context.Employee.FindAsync(document);
            if (employee == null)
            {
                return NotFound();
            }

            DeletedEmployee deletedEmployee = new(employee);

            _context.Employee.Remove(employee);
            _context.DeletedEmployee.Add(deletedEmployee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{manager}/aprove/{customer}")]
        public async Task<IActionResult> AcceptAccountRequest(string customer, string manager)
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
                    var customerAccepted = await customerApi.Patch(customer);

                    return customerAccepted.Result;
                }
                else
                    return Problem("This employee not is a manager");
            }catch(Exception ex)
            {
                return NoContent();
            }
        }

        [HttpPost("{manager}/create/account")]
        public async Task<ActionResult<Account>> AcceptAccountRequest(string manager, AccountDTO accountDTO)
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
                    var accountApi = new ApiConsumer<Account>("https://localhost:7285/api/Accounts");
                    var accountCreated = await accountApi.Post("", accountDTO);

                    return accountCreated;
                }
                else
                    return Problem("This employee not is a manager");
            }
            catch (Exception ex)
            {
                return NoContent();
            }
        }

        private bool EmployeeExists(string document)
        {
            return (_context.Employee?.Any(e => e.Document == document)).GetValueOrDefault();
        }
        private async Task<Employee> BuildEmployee(EmployeeDTO employeeDTO)
        {
            string addressId = employeeDTO.AddressDTO.ZipCode + employeeDTO.AddressDTO.Number;

            Address address = await _addressService.GetAddressByAPI(addressId);

            if (address == null)
                address = await _addressService.PostAddress(employeeDTO.AddressDTO);

            Employee employee = new Employee(employeeDTO);
            employee.Address = address;

            return employee;
        }
    }
}
