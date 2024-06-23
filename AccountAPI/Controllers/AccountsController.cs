using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

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

        return await _context.Account.ToListAsync();
    }

    // GET: api/Accounts/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Account>> GetAccount(string id)
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

        return account;
    }

    // PUT: api/Accounts/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAccount(string id, Account account)
    {
        if (id != account.Number)
        {
            return BadRequest();
        }

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
    public async Task<ActionResult<Account>> PostAccount(Account account)
    {
        if (_context.Account == null)
        {
            return Problem("Entity set 'AccountsApiContext.Account'  is null.");
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

    // DELETE: api/Accounts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAccount(string id)
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

        _context.Account.Remove(account);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AccountExists(string id)
    {
        return (_context.Account?.Any(e => e.Number == id)).GetValueOrDefault();
    }
}