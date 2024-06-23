using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OperationsController : ControllerBase
{
    private readonly AccountsApiContext _context;

    public OperationsController(AccountsApiContext context)
    {
        _context = context;
    }


    // GET: api/Operations
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // GET: api/Operations/5
    [HttpGet("{id}", Name = "Get")]
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