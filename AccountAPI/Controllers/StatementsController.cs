using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.EntityFrameworkCore;
using Models.DTO;

namespace AccountAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatementsController : ControllerBase
{
    private readonly AccountsApiContext _context;
    private readonly StatementService _statementService;

    public StatementsController(AccountsApiContext context, StatementService statementService)
    {
        _context = context;
        _statementService = statementService;
    }

    [HttpGet("All/{number}")]
    public async Task<ActionResult<IEnumerable<OperationDTO>>> GetByAccoutNumber(string number)
    {
        var operations = new List<OperationDTO>();
        if (_context.Operation == null)
        {
            return NotFound();
        }

        var result = await _context.Operation
            .Include(a => a.Account)
            .Where(n => n.Account.Number == number)
            .ToListAsync();
        foreach (var operation in result)
        {
            if (operation.Account == null)
            {
                continue;
            }

            var dto = new OperationDTO()
            {
                Id = operation.Id,
                Type = operation.Type,
                AccountNumber = operation.Account.Number,
                Value = operation.Value
            };
            operations.Add(dto);
        }

        return operations;
    }
}