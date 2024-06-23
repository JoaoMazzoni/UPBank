using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgencyAPI.Data;
using Models;
using AgencyAPI.Services;
using Models.DTO;
using NuGet.Protocol;

namespace AgencyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgenciesController : ControllerBase
    {
        private readonly AgencyAPIContext _context;
        private readonly AddressService _addressService;
        private readonly EmployeeService _employeeService;

        public AgenciesController(AgencyAPIContext context, AddressService address, EmployeeService employee)
        {
            _context = context;
            _addressService = address;
            _employeeService = employee;
        }

        // POST: api/Agencies
        [HttpPost]
        public async Task<ActionResult<Agency>> PostAgency(AgencyDTO agencyDTO)
        {
            Agency agency = new();
            List<Employee> employees = new();

            if (_context.Agency == null)
            {
                return Problem("Entity set 'AgencyAPIContext.Agency'  is null.");
            }

            foreach (var employee in agencyDTO.Employees)
            {
                if (employee == null)
                    return BadRequest("Employee not found.");
                else
                {
                    employees.Add(await _employeeService.GetEmployee(employee));
                }
            }
            
            if (employees == null)
                return BadRequest("Employee not found.");
            
            else if (!(employees.Find(e => e.Manager).Manager))
                return BadRequest("The first employee must be a manager.");
            
            else
                agency.Employees = employees;

            Address address = await _addressService.GetAddress(agencyDTO.Address.ZipCode);
            address.Number = agencyDTO.Address.Number;
            address.Complement = agencyDTO.Address.Complement;

            if (address == null)
                return BadRequest("Address not found.");
            
            else
                agency.Address = address;
                
            _context.Agency.Add(agency);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (AgencyExists(agency.Number))
                {
                    return Conflict();
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }

            return CreatedAtAction("GetAgency", new { id = agency.Number }, agency);
        }













        // PUT: api/Agencies/5
        [HttpPut("{number}")]
        public async Task<IActionResult> PutAgency(string number, AgencyPatchDTO agencyPatchDTO)
        {
            var agency = await _context.Agency.FindAsync(number);

            if (number != agency.Number)
                return BadRequest("The agency number is invalid.");

            //if (agencyPatchDTO.Address != null)
            //    agency.Address = await _addressService.PostAddress(agencyPatchDTO.Address);

            //if (agencyDTO.Employees != null)
            //{
            //    agency.Employees = await _employeeService.PostEmployee(agencyDTO.Employees);
            //}

            _context.Update(agency);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AgencyExists(number))
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


















        // GET: api/Agencies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Agency>>> GetAgency()
        {
            if (_context.Agency == null)
            {
                return NotFound();
            }
            return await _context.Agency.ToListAsync();
        }

        // GET: api/Agencies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Agency>> GetAgency(string id)
        {
            if (_context.Agency == null)
            {
                return NotFound();
            }
            var agency = await _context.Agency.FindAsync(id);

            if (agency == null)
            {
                return NotFound();
            }

            return agency;
        }




        // DELETE: api/Agencies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgency(string id)
        {
            if (_context.Agency == null)
            {
                return NotFound();
            }
            var agency = await _context.Agency.FindAsync(id);
            if (agency == null)
            {
                return NotFound();
            }

            _context.Agency.Remove(agency);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AgencyExists(string id)
        {
            return (_context.Agency?.Any(e => e.Number == id)).GetValueOrDefault();
        }
    }
}
