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
            return BadRequest("Agência não emcontrada ou dispõe de restrições.");
        }

        var customer = await _accountService.GetCustomerData(account.MainCustomerId);
        if (customer == null)
        {
            return BadRequest("Cliente não encontrado.");
        }

        account.Profile = _accountService.GetProfileBySalary(customer.Salary);
        account.SpecialLimit = _accountService.GetSpecialLimitBySalary(customer.Salary);
        account.CreditCard =
            _accountService.GenerateCreditCard(account.Profile, customer.Name);
        if (account.CreditCard == null)
        {
            return BadRequest("Informações do cliente inválidas.");
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
            return Unauthorized("Nível insuficiente de acesso para a operação.");
        }

        var account = await _context.Account.FindAsync(activateAccountRequest.AccountNumber);
        if (account == null)
        {
            return NotFound();
        }

        if (account.MainCustomerId != activateAccountRequest.CustomerDocument)
        {
            return BadRequest("O documento do cliente não corresponde ao da conta solicitada.");
        }

        if (!account.Restriction)
        {
            return BadRequest("A conta já se encontra ativada.");
        }

        account.Restriction = false;
        await _context.SaveChangesAsync();

        return Ok("Conta ativada com sucesso.");
    }

    [HttpPatch("ActivateCard/{numberAccount}")]
    public async Task<ActionResult<Account>> ActivateCard(string numberAccount)
    {
        var account = await _context.Account.Include(e => e.CreditCard).FirstOrDefaultAsync(ac => ac.Number == numberAccount);
        var card = account.CreditCard.Number;
        var cardExists = await _context.CreditCard.FirstOrDefaultAsync(c => c.Number == card);

        if (account == null)
        {
            return NotFound();
        }
        if(cardExists == null)
        {
            return NotFound();
        }
        else
        {
            cardExists.Active = true;
            _context.Entry(cardExists).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        return account;
    }

    [HttpPatch("DisableCard/{numberAccount}")]
    public async Task<ActionResult<Account>> DeactivateCard(string numberAccount)
    {
        var account = await _context.Account.Include(e => e.CreditCard).FirstOrDefaultAsync(ac => ac.Number == numberAccount);
        var card = account.CreditCard.Number;
        var cardExists = await _context.CreditCard.FirstOrDefaultAsync(c => c.Number == card);

        if (account == null)
        {
            return NotFound();
        }
        if (cardExists == null)
        {
            return NotFound();
        }
        else
        {
            cardExists.Active = false;
            _context.Entry(cardExists).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        return account;
    }


    // DELETE: api/Accounts/
    [HttpDelete]
    public async Task<ActionResult<AccountDTO>> RestrictAccount(RestrictAccountDTO restrictAccountRequest)
    {
        if (_context.Account == null)
        {
            return Problem("Entity set 'AccountsApiContext.Account'  is null.");
        }

        restrictAccountRequest.CustomerDocument = CPFValidator.FormatCPF(restrictAccountRequest.CustomerDocument);
        var isManagerRequest = await _accountService.ValidateManagerRequest(restrictAccountRequest.EmployeeId);
        if (!isManagerRequest)
        {
            return Unauthorized("Nível insuficiente de acesso para a operação.");
        }

        var accountToRestrict =
            await _context.Account.FirstOrDefaultAsync(ac => ac.Number == restrictAccountRequest.AccountNumber);
        if (accountToRestrict == null)
        {
            return NotFound();
        }

        if (accountToRestrict.MainCustomerId != restrictAccountRequest.CustomerDocument)
        {
            return BadRequest("O documento do cliente não corresponde ao da conta solicitada.");
        }

        // Restringir a conta
        accountToRestrict.Restriction = true;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            if (AccountExists(accountToRestrict.Number))
            {
                return Conflict();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    private bool AccountExists(string id)
    {
        return (_context.Account?.Any(e => e.Number == id)).GetValueOrDefault();
    }
}