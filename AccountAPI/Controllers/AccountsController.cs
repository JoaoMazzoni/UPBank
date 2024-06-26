using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO;
using Models.Utils;
using NuGet.Protocol;

namespace AccountAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly AccountsApiContext _context;
    private readonly AccountService _accountService;

    public AccountsController(AccountsApiContext context, AccountService accountService)
    {
        _context = context;
        _accountService = accountService;
    }

    // GET: api/Accounts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Account>>> GetAccount()
    {
        if (_context.Account == null)
        {
            return NotFound();
        }

        return await _context.Account.Include(ac => ac.CreditCard).ToListAsync();
    }

    // GET: api/Accounts/5
    [HttpGet("{accountNumber}")]
    public async Task<ActionResult<Account>> GetAccount(string accountNumber)
    {
        if (_context.Account == null)
        {
            return NotFound();
        }

        var account = await _context.Account
            .Include(ac => ac.CreditCard)
            .Where(ac => ac.Number == accountNumber)
            .FirstOrDefaultAsync();

        if (account == null)
        {
            return NotFound();
        }

        return account;
    }

    // PUT: api/Accounts/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAccount(string id, AccountDTO accountDto)
    {
        if (id != accountDto.Number)
        {
            return BadRequest();
        }

        var account = new Account(accountDto);
        _context.Entry(account).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AccountExists(id))
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

    // POST: api/Accounts
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<AccountDTO>> PostAccount(AccountInsertDTO accountDto)
    {
        if (_context.Account == null)
        {
            return Problem("Entity set 'AccountsApiContext.Account'  is null.");
        }

        var account = new Account(accountDto);
        var agencyStatus = await _accountService.CheckAgencyStatus(account.AgencyNumber);
        if (!agencyStatus)
        {
            return BadRequest("Agency has restriction or doesnt exist.");
        }

        var customer = await _accountService.GetCustomerData(account.MainCustomerId);
        if (customer == null)
        {
            return BadRequest("Customer not found.");
        }

        account.Profile = _accountService.GetProfileBySalary(customer.Salary);
        account.SpecialLimit = _accountService.GetSpecialLimitBySalary(customer.Salary);
        account.CreditCard =
            _accountService.GenerateCreditCard(account.Profile, customer.Name);
        if (account.CreditCard == null)
        {
            return BadRequest("Invalid information retrieved from '/api/Customer'");
        }

        _context.Account.Add(account);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            if (AccountExists(account.Number))
            {
                return Conflict();
            }
            else
            {
                throw;
            }
        }

        return CreatedAtAction("GetAccount", new { id = account.Number }, account);
    }

    // POST: api/Accounts/Activate
    [HttpPost("Activate")]
    public async Task<ActionResult<AccountDTO>> ActivateAccount(ActivateAccountDTO activateAccountRequest)
    {
        activateAccountRequest.CustomerDocument = CPFValidator.FormatCPF(activateAccountRequest.CustomerDocument);
        if (_context.Account == null)
        {
            return Problem("Entity set 'AccountsApiContext.Account'  is null.");
        }

        var isManagerRequest = await _accountService.ValidateManagerRequest(activateAccountRequest.EmployeeId);
        if (!isManagerRequest)
        {
            return Unauthorized("Access level denied.");
        }

        var account = await _context.Account.FindAsync(activateAccountRequest.AccountNumber);
        if (account == null)
        {
            return NotFound();
        }

        if (account.MainCustomerId != activateAccountRequest.CustomerDocument)
        {
            return BadRequest("Customer document doesn't match target account.");
        }

        if (!account.Restriction)
        {
            return BadRequest("Account already activated.");
        }

        account.Restriction = false;
        await _context.SaveChangesAsync();

        return Ok("Account activated successfully.");
    }

    // POST: api/Accounts/Recover
    [HttpPost("Recover")]
    public async Task<ActionResult<AccountDTO>> RecoverAccount(RecoverAccountDTO recoverAccountRequest)
    {
        recoverAccountRequest.CustomerDocument = CPFValidator.FormatCPF(recoverAccountRequest.CustomerDocument);
        if (_context.Account == null)
        {
            return Problem("Entity set 'AccountsApiContext.Account'  is null.");
        }

        var isManagerRequest = await _accountService.ValidateManagerRequest(recoverAccountRequest.EmployeeId);
        if (!isManagerRequest)
        {
            return Unauthorized("Access level denied.");
        }

        var disabledAccount = await _context.DisabledAccount.FindAsync(recoverAccountRequest.AccountNumber);
        if (disabledAccount == null)
        {
            return NotFound();
        }

        if (disabledAccount.MainCustomerId != recoverAccountRequest.CustomerDocument)
        {
            return BadRequest("Customer document doesn't match target account.");
        }

        var enabledAccount = _accountService.EnableAccountFeatures(disabledAccount);
        await _context.Account.AddAsync(enabledAccount);
        _context.DisabledAccount.Remove(disabledAccount);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            if (AccountExists(enabledAccount.Number))
            {
                return Conflict();
            }
            else
            {
                throw;
            }
        }

        return CreatedAtAction("GetAccount", new { id = enabledAccount.Number }, enabledAccount);
    }

    // DELETE: api/Accounts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DisableAccount(string id)
    {
        if (_context.Account == null)
        {
            return NotFound();
        }

        var account = await _context.Account.FindAsync(id);
        if (account == null)
        {
            return NotFound();
        }

        _context.DisabledAccount.Add(_accountService.DisableAccountFeatures(account));
        _context.Account.Remove(account);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AccountExists(string id)
    {
        return (_context.Account?.Any(e => e.Number == id)).GetValueOrDefault();
    }
}