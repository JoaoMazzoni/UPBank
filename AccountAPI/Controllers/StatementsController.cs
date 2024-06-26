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

    [HttpGet("{number}")]
    public async Task<ActionResult<IEnumerable<OperationDTO>>> GetByAccoutNumber(string number)
    {
        List<OperationDTO> operations = new();
        if (_context.Operation == null)
        {
            return NotFound();
        }

        var result = await _context.OperationAccount.Include(oa => oa.Operation).ThenInclude(a => a.Account)
            .Where(oa => oa.AccountId == number).ToListAsync();
        foreach (var operationAccount in result)
        {
            var dto = new OperationDTO()
            {
                Id = operationAccount.Operation.Id,
                Type = operationAccount.Operation.Type,
                Value = operationAccount.Operation.Value,
                Date = operationAccount.Operation.Date
            };
            if (operationAccount.Operation.Account is not null)
            {
                dto.TargetAccountNumber = operationAccount.Operation.Account.Number;
            }

            operations.Add(dto);
        }

        return operations;
    }
}