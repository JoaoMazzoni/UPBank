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
        private readonly AccountService _accountService;
        public AgenciesController(AgencyAPIContext context, AddressService address, EmployeeService employee, AccountService account)
        {
            _context = context;
            _addressService = address;
            _employeeService = employee;
            _accountService = account;
        }

        // POST: api/Agencies
        [HttpPost]
        public async Task<ActionResult<Agency>> PostAgency(AgencyDTO agencyDTO)
        {
            Agency agency = new();
            List<Employee> employees = new();

            if (_context.Agency == null)
                return Problem("Entity set 'AgencyAPIContext.Agency'  is null.");

            if (!Agency.VerifyCNPJ(agencyDTO.CNPJ))
                return BadRequest("CNPJ inválido!");

            foreach (var employee in agencyDTO.Employees)
            {
                if (employee == null)
                    return BadRequest("Funcionario não foi achado.");
                else
                {
                    var ifEmployeeExistInAgencies = await _employeeService.GetEmployeeOnAgency(employee);
                    if (ifEmployeeExistInAgencies == null)
                        employees.Add(await _employeeService.GetEmployee(employee));
                }
            }

            if (employees == null)
                return BadRequest("Funcionario não encontrado!");

            else if (!(employees.Find(e => e.Manager).Manager))
                return BadRequest("É necessário ter um gerente na agencia!");

            else
                agency.Employees = employees;


            Address address = await _addressService.PostAddress(agencyDTO.Address);

            address.Number = agencyDTO.Address.Number;
            address.Complement = agencyDTO.Address.Complement;

            if (address == null)
                return BadRequest("Endereço não encontrado!");

            else
            {
                agency.Address = address;
                agency.AddressId = address.Id;
            }

            agency.Number = agencyDTO.Number;
            agency.Restriction = agencyDTO.Restriction;
            agency.CNPJ = agencyDTO.CNPJ;

            _context.Agency.Add(agency);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (AgencyExists(agency.Number))
                {
                    return Conflict("Numero da agencia já existe!");
                }
                else
                {
                    return BadRequest("Houve um erro inesperado: " + e.Message);
                }
            }
            return CreatedAtAction("GetAgency", new { id = agency.Number }, agency);
        }

        // PUT: api/Agencies/5
        [HttpPut("{number}")]
        public async Task<IActionResult> PutAgency(string number, AgencyPatchDTO agencyPatchDTO)
        {
            var agencyGet = await GetAgency(number);
            var agency = agencyGet.Value;

            if (!agency.Restriction)
            {
                if (number != agency.Number)
                    return BadRequest("O numero da agencia não foi encontrado");

                if ((agency.Address.ZipCode != agencyPatchDTO.Address.ZipCode && agencyPatchDTO.Address.ZipCode != "") || (agency.Address.Number != agencyPatchDTO.Address.Number && agencyPatchDTO.Address.Number != 0) || (agency.Address.Complement != agencyPatchDTO.Address.Complement && agencyPatchDTO.Address.Complement != ""))
                {
                    agency.Address = await _addressService.GetAddress(agencyPatchDTO.Address.ZipCode);
                    agency.Address.Number = agencyPatchDTO.Address.Number;
                    agency.Address.Complement = agencyPatchDTO.Address.Complement;
                    await _addressService.PutAddress(agencyPatchDTO.Address.ZipCode, agencyPatchDTO.Address);
                }

                if (agencyPatchDTO.Employees != null)
                {
                    foreach (var employee in agencyPatchDTO.Employees)
                        agency.Employees.Add(await _employeeService.GetEmployee(employee));
                }

                _context.Update(agency);
                try
                {
                    await _context.SaveChangesAsync();
                    return Ok(agency);
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (!AgencyExists(number))
                    {
                        return NotFound("Agency não encontrada");
                    }
                    else
                    {
                        return BadRequest("Houve um erro inesperado:" + e.Message);
                    }
                }
            }

            else
            {
                return BadRequest("The agency is restricted.");
            }
        }

        // PUT: api/Agencies/5
        [HttpPut("Restricted/{number}")]
        public async Task<IActionResult> PutRestrictedAgency(string number)
        {
            var agencyGet = await GetAgency(number);
            var agency = agencyGet.Value;
            if (agency.Restriction)
                agency.Restriction = false;
            else
                agency.Restriction = true;

            _context.Update(agency);
            try
            {
                await _context.SaveChangesAsync();
                return Ok(agency);
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (!AgencyExists(number))
                {
                    return NotFound("Agency não encontrada.");
                }
                else
                {
                    return BadRequest("Houve um erro inesperado" + e.Message);
                }
            }
        }

        // GET: api/Agencies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Agency>>> GetAgency()
        {
            if (_context.Agency == null)
            {
                return Ok("Não há agencias!");
            }
            var agencies = await _context.Agency.Include(e => e.Employees).ToListAsync();
            foreach (var agency in agencies)
            {
                agency.Address = await _addressService.GetAddress(agency.AddressId);

                var employees = agency.Employees;
                foreach (var employee in employees)
                {
                    employee.Address = await _addressService.GetAddress(employee.AddressId);
                }
            }

            return Ok(agencies);
        }

        // GET: api/Agencies/5
        [HttpGet("{number}")]
        public async Task<ActionResult<Agency>> GetAgency(string number)
        {
            if (_context.Agency == null)
            {
                return NotFound();
            }
            var agency = await _context.Agency.Include(e => e.Employees).Where(c => c.Number == number).SingleOrDefaultAsync();

            if (agency == null)
                return NotFound("Agencia não foi encontrada@");
            else
            {
                agency.Address = await _addressService.GetAddress(agency.AddressId);

                var employees = agency.Employees;

                foreach (var employee in employees)
                    employee.Address = await _addressService.GetAddress(employee.AddressId);
            }
            return Ok(agency);
        }

        // DELETE: api/Agencies/5
        [HttpDelete("Delete/{number}")]
        public async Task<IActionResult> DeleteAgency(string number)
        {
            if (_context.Agency == null)
            {
                return NotFound("Agencia não encontada!");
            }
            var agency = await _context.Agency.FindAsync(number);
            var agencyDb = await GetAgency(number);
            agencyDb = agencyDb.Value;

            List<RemovedAgencyEmployee> employees = new();

            foreach (var employee in agencyDb.Value.Employees)
            {
                var emp = await _employeeService.GetEmployee(employee.Document);
                var employeeNew = new RemovedAgencyEmployee
                {
                    AddressId = emp.AddressId,
                    Name = emp.Name,
                    BirthDate = emp.BirthDate,
                    Document = emp.Document,
                    Email = emp.Email,
                    Phone = emp.Phone,
                    Gender = emp.Gender,
                    Manager = emp.Manager,
                    Register = emp.Register,
                    Address = emp.Address,
                    Salary = emp.Salary
                };
                employees.Add(employeeNew);
            }

            Address address = await _addressService.GetAddressById(agency.AddressId);


            var agencyCopied = new RemovedAgency
            {
                Number = agency.Number,
                AddressId = agency.AddressId,
                Address = address,
                Employees = employees,
                CNPJ = agency.CNPJ,
                Restriction = agency.Restriction
            };

            _context.AgencyHistory.Add(agencyCopied);
            _context.Agency.Remove(agency);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {

                return BadRequest("Houve um erro inesperado" + e.Message);
            }
            return Ok("Deletado com Sucesso!");
        }

        // DELETE: api/Agencies/Restorage/{number}
        [HttpDelete("Restorage/{number}")]
        public async Task<IActionResult> RestorageAgency(string number)
        {
            var agencyDb = await _context.AgencyHistory.Include(e => e.Employees).Where(c => c.Number == number).SingleOrDefaultAsync();

            if (agencyDb == null)
                return NotFound("Agencia não encontada!");
            else
            {
                agencyDb.Address = await _addressService.GetAddress(agencyDb.AddressId);
                List<Employee> employees = new();
                foreach (var employee in employees)
                {
                    employee.Address = await _addressService.GetAddress(employee.AddressId);
                    var employeesNew = new Employee
                    {
                        AddressId = employee.AddressId,
                        Name = employee.Name,
                        BirthDate = employee.BirthDate,
                        Document = employee.Document,
                        Email = employee.Email,
                        Phone = employee.Phone,
                        Gender = employee.Gender,
                        Manager = employee.Manager,
                        Register = employee.Register,
                        Address = employee.Address,
                        Salary = employee.Salary
                    };
                    employees.Add(employeesNew);
                }

                var agencyCopied = new Agency
                {
                    Number = agencyDb.Number,
                    AddressId = agencyDb.AddressId,
                    Address = agencyDb.Address,
                    Employees = employees,
                    CNPJ = agencyDb.CNPJ,
                    Restriction = agencyDb.Restriction
                };

                _context.Agency.Add(agencyCopied);
                _context.AgencyHistory.Remove(agencyDb);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    return BadRequest("Houve um erro inesperado:" + e.Message);
                }
                return Ok("Restauração concluida!");
            }
        }

        private bool AgencyExists(string id)
        {
            return (_context.Agency?.Any(e => e.Number == id)).GetValueOrDefault();
        }

        //Get: api/Agencies/RestrictedAccounts
        [HttpGet("RestrictAccounts")]
        public async Task<ActionResult<IEnumerable<Account>>> GetRestrictedAccounts()
        {
            return await _accountService.GetRestrictedAccounts();
        }

        //Get: api/Agencies/AccountsPerProfile
        [HttpGet("AccountsPerProfile/{profile}")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccountsPerProfile(string profile)
        {
            return await _accountService.GetAccountsPerProfile(profile);
        }



        //Get: api/Agencies/ActiveLoan
        [HttpGet("CustomerWithActiveLoan")]

        public async Task<ActionResult<IEnumerable<Account>>> GetActiveLoan()
        {
            return await _accountService.GetActiveLoan();
        }

    }
}
