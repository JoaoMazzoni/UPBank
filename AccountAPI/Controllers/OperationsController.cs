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
using Type = Models.Type;

namespace AccountAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OperationsController : ControllerBase
{
    private readonly AccountsApiContext _context;
    private readonly OperationService _operationService;

    public OperationsController(AccountsApiContext context, OperationService operationService)
    {
        _context = context;
        _operationService = operationService;
    }


    [HttpGet("type/{type}", Name = "GetByType")]
    public async Task<ActionResult<IEnumerable<OperationDTO>>> GetByType(int type)
    {
        List<OperationDTO> operations = new List<OperationDTO>();
        var result = await _context.Operation.Include(a => a.Account).Where(t => t.Type == (Type)type).ToListAsync();
        if (_context.Operation == null)
        {
            return NotFound();
        }
        if (result == null)
        {
            return NotFound();
        }
        if (result.Count == 0)
        {
            return Problem("Nenhuma transação com o tipo definido encontrada no banco");
        }

        foreach (var operation in result)
        {
            var dto = new OperationDTO()
            {
                Id = operation.Id,
                Type = (Type)type,
                AccountNumber = operation.Account.Number,
                Date = operation.Date,
                Value = operation.Value
            };
            operations.Add(dto);
        }
        return operations;
    }
    // GET: api/Operations/5
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OperationDTO>>> GetOperation()
    {
        List<OperationDTO> operations = new List<OperationDTO>();
        if (_context.Operation == null)
        {
            return NotFound();
        }
        var result = await _context.Operation.Include(a => a.Account).ToListAsync();
        foreach (Operation operation in result)
        {
            if (operation.Account == null)
            {
                continue;
            }
            OperationDTO dto = new OperationDTO()
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
   [HttpGet("number/{number}")]
   public async Task<ActionResult<IEnumerable<OperationDTO>>> GetByNumber(string number)
    {
        List<OperationDTO> operations = new List<OperationDTO>();
        if (_context.Operation == null)
        {
            return NotFound();
        }
        var result = await _context.Operation.Include(a => a.Account).Where(n => n.Account.Number == number).ToListAsync();
        foreach (Operation operation in result)
        {
            if (operation.Account == null)
            {
                continue;
            }
            OperationDTO dto = new OperationDTO()
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
    // POST: api/Operations
    [HttpPost]
    public async Task<ActionResult<OperationDTO>> PostOperation(OperationDTO dto)
    {
        Operation operation = _operationService.GenerateOperation(dto);
        operation.Account = await _context.Account.FindAsync(dto.AccountNumber);
        OperationAccount ac = new OperationAccount()
        {
            AccountId = operation.Account.Number,
            Account = operation.Account,
            OperationId = operation.Id,
            Operation = operation
     
        };
        if((int)dto.Type >= 5 || (int) dto.Type < 0)
        {
            return Problem("Tipo de transação inválida");
        }
        if (operation == null)
        {
            return NotFound();
        }
        _context.OperationAccount.Add(ac);
        await _context.SaveChangesAsync();
        return CreatedAtAction("GetOperation", new { id = 0 }, dto);
    }
    

    // PUT: api/Operations/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE: api/Operations/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}