using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;

namespace AccountAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BalancesController : ControllerBase
{
    private readonly AccountsApiContext _context;
    private readonly BalanceService _balanceService;

    public BalancesController(AccountsApiContext context, BalanceService balanceService)
    {
        _context = context;
        _balanceService = balanceService;
    }

    // GET: api/Balances/5
    [HttpGet("{accountNumber}")]
    public async Task<ActionResult<BalanceDTO>> GetCurrentBalance(string? accountNumber)
    {
        if (_context.Account == null)
        {
            return NotFound();
        }

        if (accountNumber == null)
        {
            return BadRequest("O argumento 'targetAccountNumber' é necessário.");
        }

        var account = await _context.Account.FindAsync(accountNumber);
        if (account == null)
        {
            return NotFound();
        }

        return Ok(_balanceService.PopulateBalanceDto(account));
    }
}