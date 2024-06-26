﻿using Microsoft.AspNetCore.Mvc;
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
        private readonly IAddressService _addressService;
        private readonly IEmployeeService _employeeService;
        private readonly IAccountService _accountService;
        private readonly IOperationService _operationService;

        public AgenciesController(AgencyAPIContext context, IAddressService address, IEmployeeService employee, IAccountService account, IOperationService operationService)
        {
            _context = context;
            _addressService = address;
            _employeeService = employee;
            _accountService = account;
            _operationService = operationService;
        }

        // POST: api/Agencies
        [HttpPost]
        public async Task<ActionResult<Agency>> PostAgency(AgencyDTO agencyDTO)
        {
            Agency agency = new();
            List<Employee> employees = new();

            var deleted = await _context.AgencyHistory.FirstOrDefaultAsync(e => e.Number == agencyDTO.Number);

            if (_context.Agency == null)
                return Problem("Entity set 'AgencyAPIContext.Agency'  is null.");
            else
            {
                if (deleted != null)
                    BadRequest("A agencia foi deletada! Restaure ela");
            }

            agencyDTO.CNPJ = Agency.RemoveMask(agencyDTO.CNPJ);

            if (!Agency.VerifyCNPJ(agencyDTO.CNPJ))
                return BadRequest("CNPJ inválido!");
            else
            {
                var existCnpj = await _context.Agency.AnyAsync(e => e.CNPJ == agencyDTO.CNPJ);
                if (existCnpj)
                    return BadRequest("CNPJ já cadastrado!");
            }
            agencyDTO.CNPJ = Agency.InsertMask(agencyDTO.CNPJ);

            foreach (var employee in agencyDTO.Employees)
            {
                if (employee == null)
                    return BadRequest("Funcionario não foi encontrado.");
                else
                {
                    var ifEmployeeExistInAgencies = await _context.Employee.AnyAsync(e => e.Document == employee);


                    if (!ifEmployeeExistInAgencies)
                    {
                        var ifEmployeeExistInAgenciesHistory = await _context.RemovedAgencyEmployee.AnyAsync(e => e.Document == employee);
                        if (ifEmployeeExistInAgenciesHistory)
                        {
                            _context.RemovedAgencyEmployee.Remove(await _context.RemovedAgencyEmployee.FirstOrDefaultAsync(e => e.Document == employee));
                        }
                        employees.Add(await _employeeService.GetEmployee(employee));
                    }

                    else
                    {
                        return BadRequest("Funcionario ja cadastrado em alguma agencia!");
                    }
                }
            }

            var manager = employees.Find(e => e.Manager);
            if (manager == null)
                return BadRequest("É necessário ter um gerente na agencia!");

            else
                agency.Employees = employees;

            Address address = await _addressService.PostAddress(agencyDTO.Address);

            agency.Address = address;
            agency.AddressId = agencyDTO.Address.ZipCode + agencyDTO.Address.Number;

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
            var agency = await _context.Agency.Include(e => e.Employees).Where(c => c.Number == number).SingleOrDefaultAsync();

            if (!agency.Restriction)
            {
                if (number != agency.Number)
                    return BadRequest("O numero da agencia não foi encontrado");

                var address = await _addressService.GetAddressById(agency.AddressId);
                agency.Address = address;

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
                    {
                        var ifEmployeeExistInAgencies = await _context.Employee.AnyAsync(e => e.Document == employee);

                        if (!ifEmployeeExistInAgencies)
                        {
                            var ifEmployeeExistInAgenciesHistory = await _context.RemovedAgencyEmployee.AnyAsync(e => e.Document == employee);
                            if (ifEmployeeExistInAgenciesHistory)
                            {
                                _context.RemovedAgencyEmployee.Remove(await _context.RemovedAgencyEmployee.FirstOrDefaultAsync(e => e.Document == employee));
                            }
                            agency.Employees.Add(await _employeeService.GetEmployee(employee));
                        }

                        else
                            return BadRequest("Funcionario ja cadastrado em alguma agencia!");
                    }
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
                        return NotFound("Agency não encontrada");
                    else
                        return BadRequest("Houve um erro inesperado:" + e.Message);
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
            var agency = await _context.Agency.Include(e => e.Employees).Where(c => c.Number == number).SingleOrDefaultAsync();

            var address = await _addressService.GetAddressById(agency.AddressId);
            agency.Address = address;

            foreach (var employee in agency.Employees)
            {
                employee.Address = await _addressService.GetAddressById(employee.AddressId);
            }

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
                agency.Address = await _addressService.GetAddressById(agency.AddressId);

                var employees = agency.Employees;
                foreach (var employee in employees)
                {
                    employee.Address = await _addressService.GetAddressById(employee.AddressId);
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
                return NotFound("Agencia não foi encontrada!");
            else
            {
                agency.Address = await _addressService.GetAddress(agency.AddressId);

                var employees = agency.Employees;

                foreach (var employee in employees)
                    employee.Address = await _addressService.GetAddressById(employee.AddressId);
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
            var employees = await _context.Employee.ToListAsync();

            List<RemovedAgencyEmployee> employeesOnAgency = new List<RemovedAgencyEmployee>();

            foreach (var emp in employees)
            {
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
                employeesOnAgency.Add(employeeNew);
                _context.Employee.Remove(emp);
            }

            Address address = await _addressService.GetAddressById(agency.AddressId);

            var agencyCopied = new RemovedAgency
            {
                Number = agency.Number,
                AddressId = agency.AddressId,
                Address = address,
                Employees = employeesOnAgency,
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
                foreach (var employee in agencyDb.Employees)
                {
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
                    ; _context.RemovedAgencyEmployee.Remove(employee);
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
            var restrictedAccounts = await _accountService.GetRestrictedAccounts();
            if (restrictedAccounts == null)
            {
                return NotFound("Não há contas restritas");
            }
            else
            {
                return Ok(restrictedAccounts);
            }

        }

        //Get: api/Agencies/AccountsPerProfile
        [HttpGet("AccountsPerProfile/{profile}")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccountsPerProfile(string profile)
        {
            var acconts = await _accountService.GetAccountsPerProfile(profile);
            if (acconts == null)
            {
                return NotFound("Não há contas com esse perfil");
            }
            else
            {
                return Ok(acconts);
            }
        }

        //Get: api/Agencies/ActiveLoan
        [HttpGet("ActiveLoan")]

        public async Task<ActionResult<IEnumerable<Operation>>> GetActiveLoan()
        {
            var loans = await _operationService.GetOperationsByTypeLoan();
            if (loans == null)
            {
                return NotFound("Não há contas com emprestimo");
            }
            else
            {
                return Ok(loans);
            }
        }

    }
}
