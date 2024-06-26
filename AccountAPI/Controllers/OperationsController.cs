using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountAPI.Services;
using Humanizer;
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

    //GET: api/Operations/001/2
    [HttpGet("{Account}/{type}")]
    public async Task<ActionResult<IEnumerable<OperationDTO>>> GetByType(string Account, string Type)
    {
        if (!Enum.TryParse(Type, true, out Type type))
        {
            return BadRequest("Tipo de transação inválido");
        }

        List<OperationDTO> operations = new();
        if (_context.Operation == null)
        {
            return NotFound();
        }

        var accountExists = await _context.Account.AnyAsync(a => a.Number == Account);
        if (!accountExists)
        {
            return NotFound("Conta não encontrada");
        }

        var result = await _context.OperationAccount.Include(oa => oa.Operation)
            .ThenInclude(a => a.Account)
            .Where(oa => oa.Operation.Type == type && oa.AccountId == Account)
            .ToListAsync();


        if (result.Count == 0)
        {
            return Problem("Não foi encontrado nenhum resultado para o tipo de transação escolhida");
        }

        foreach (var operationAccount in result)
        {
            var dto = new OperationDTO()
            {
                Id = operationAccount.Operation.Id,
                Type = operationAccount.Operation.Type,
                Value = operationAccount.Operation.Value,
                Date = operationAccount.Operation.Date
            };
            if (operationAccount.Operation.Account != null)
            {
                dto.TargetAccountNumber = operationAccount.Operation.Account.Number;
            }

            operations.Add(dto);
        }

        return Ok(operations);
    }

    //GET: api/Operations/type/2
    [HttpGet("type/{type}")]
    public async Task<ActionResult<IEnumerable<OperationDTO>>> GetByType(int type)
    {
        var operations = new List<OperationDTO>();
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
            return BadRequest("Nenhuma transação com o tipo definido encontrada no banco");
        }

        foreach (var operation in result)
        {
            var dto = new OperationDTO()
            {
                Id = operation.Id,
                Type = (Type)type,
                Date = operation.Date,
                Value = operation.Value
            };

            if (operation.Account is not null)
            {
                dto.TargetAccountNumber = operation.Account.Number;
            }

            operations.Add(dto);
        }

        return operations;
    }

    // GET: api/Operations/001
    [HttpGet("{AccountNumber}")]
    public async Task<ActionResult<IEnumerable<OperationDTO>>> GetByNumber(string AccountNumber)
    {
        List<OperationDTO> operations = new();
        if (_context.Operation == null)
        {
            return NotFound();
        }

        var result = await _context.OperationAccount.Include(oa => oa.Operation).ThenInclude(a => a.Account)
            .Where(oa => oa.AccountId == AccountNumber).ToListAsync();
        foreach (var operationAccount in result)
        {
            var dto = new OperationDTO()
            {
                Id = operationAccount.Operation.Id,
                Type = operationAccount.Operation.Type,
                Value = operationAccount.Operation.Value,
                Date = operationAccount.Operation.Date
            };
            if (operationAccount.Operation.Account != null)
            {
                dto.TargetAccountNumber = operationAccount.Operation.Account.Number;
            }

            operations.Add(dto);
        }

        return operations;
    }

    // POST: api/Operations/2/002
    [HttpPost("{Type}/{OriginalAccount}")]
    public async Task<ActionResult<OperationDTO>> PostOperation(string Type, string OriginalAccount, OperationDTO dto)
    {
        try
        {
            if (!Enum.TryParse(Type, true, out Type type))
            {
                return BadRequest("Tipo de transação inválido");
            }

            dto.Type = type;
            var account = await _context.Account.FindAsync(OriginalAccount);
            if (account == null)
            {
                return BadRequest("Conta principal não encontrada");
            }

            _operationService.CheckOperation(account, dto);
            var operation = _operationService.GenerateOperation(dto, (int)dto.Type == 3);
            //Se for transferencia procura e popula a conta de destino
            if ((int)dto.Type == 3)
            {
                operation.Account = await _context.Account.FindAsync(dto.TargetAccountNumber);
                if (operation.Account == null)
                {
                    throw new ArgumentException("Conta de destino não encontrada");
                }

                if (operation.Account.Restriction)
                {
                    throw new ArgumentException("Conta de destino possui restrição");
                }
            }

            var ac = new OperationAccount()
            {
                AccountId = OriginalAccount,
                Account = account,
                OperationId = operation.Id,
                Operation = operation
            };
            if ((int)dto.Type >= 5 || (int)dto.Type < 0)
            {
                throw new ArgumentException("Tipo de transação inválida");
            }

            if ((int)dto.Type == 3)
            {
                operation.Value *= -1;
            }

            if ((int)dto.Type == 2)
            {
                _context.Loan.Add(new Loan
                {
                    AccountNumber = account.Number,
                    CustomerDocument = account.MainCustomerId
                });
            }

            if (operation == null)
            {
                return NotFound();
            }

            _context.Operation.Add(operation);
            await _context.SaveChangesAsync();

            dto.Id = operation.Id;
            dto.Date = operation.Date;

            _context.OperationAccount.Add(ac);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(PostOperation), new { id = operation.Id }, dto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Erro = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Erro = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Ocorreu um erro interno." });
        }
    }
}