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
public class OperationsController : ControllerBase
{
    private readonly AccountsApiContext _context;
    private readonly OperationService _operationService;

    public OperationsController(AccountsApiContext context, OperationService operationService)
    {
        _context = context;
        _operationService = operationService;
    }

    // GET: api/Operations/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST: api/Operations
    [HttpPost]
    public void Post([FromBody] string value)
    {
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