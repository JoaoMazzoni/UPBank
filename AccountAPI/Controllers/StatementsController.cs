using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;

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

    // GET: api/Statements/All/0001
    [HttpGet("All/{accountNumber}")]
    public ActionResult<List<Operation>> GetByAccount()
    {
        var accountStatements = new List<Operation>();
        return accountStatements;
    }
}