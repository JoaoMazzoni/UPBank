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
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(string id)
        {
            if (_context.Employee == null)
            {
                return NotFound();
            }
            try
            {
                var employee = await _context.Employee.FindAsync(id);
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

        [HttpGet("get/customers_requests")]
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
        }

        // PUT: api/Employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(string id, Employee employee)
        {
            if (id != employee.Document)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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

            return CreatedAtAction("GetEmployee", new { id = employee.Document }, employee);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            if (_context.Employee == null)
            {
                return NotFound();
            }
            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{manager}/accept/{customer}")]
        public async Task<ActionResult<Customer>> AcceptAccountRequest(string customer, string manager)
        {
            var employee = await _context.Employee.FindAsync(manager);

            if (employee == null)
            {
                return NotFound("not was possible to find this employee, Document: " + manager);
            }
            if (employee.Manager)
            {
                var customerApi = new ApiConsumer<Customer>("https://localhost:7045/api/Customers/");
                var customerAccpted = customerApi.Patch(customer);

                return customerAccpted;
            }
            else
                return Problem("This employee not is a manager");

        }

        private bool EmployeeExists(string id)
        {
            return (_context.Employee?.Any(e => e.Document == id)).GetValueOrDefault();
        }
        private async Task<Employee> BuildEmployee(EmployeeDTO employeeDTO)
        {
            string addressId = employeeDTO.AddressDTO.ZipCode + employeeDTO.AddressDTO.Number;

            Address address = _addressService.GetAddressByAPI(addressId).Result;

            if (address == null)
                address = _addressService.PostAddress(employeeDTO.AddressDTO).Result;

            Employee employee = new Employee(employeeDTO);
            employee.Address = address;

            return employee;
        }
    }
}
