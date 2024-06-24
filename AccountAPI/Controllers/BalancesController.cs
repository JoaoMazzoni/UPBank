using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST: api/Balances
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT: api/Balances/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE: api/Balances/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}