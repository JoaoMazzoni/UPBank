using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgencyAPI.Data;
using Models;

namespace AgencyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgenciesController : ControllerBase
    {
        private readonly AgencyAPIContext _context;

        public AgenciesController(AgencyAPIContext context)
        {
            _context = context;
        }

        // GET: api/Agencies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Agency>>> GetAgency()
        {
          if (_context.Agency == null)
          {
              return NotFound();
          }
            return await _context.Agency.ToListAsync();
        }

        // GET: api/Agencies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Agency>> GetAgency(string id)
        {
          if (_context.Agency == null)
          {
              return NotFound();
          }
            var agency = await _context.Agency.FindAsync(id);

            if (agency == null)
            {
                return NotFound();
            }

            return agency;
        }

        // PUT: api/Agencies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAgency(string id, Agency agency)
        {
            if (id != agency.Number)
            {
                return BadRequest();
            }

            _context.Entry(agency).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AgencyExists(id))
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

        // POST: api/Agencies
        [HttpPost]
        public async Task<ActionResult<Agency>> PostAgency(Agency agency)
        {
          if (_context.Agency == null)
          {
              return Problem("Entity set 'AgencyAPIContext.Agency'  is null.");
          }

            _context.Agency.Add(agency);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AgencyExists(agency.Number))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAgency", new { id = agency.Number }, agency);
        }

        // DELETE: api/Agencies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgency(string id)
        {
            if (_context.Agency == null)
            {
                return NotFound();
            }
            var agency = await _context.Agency.FindAsync(id);
            if (agency == null)
            {
                return NotFound();
            }

            _context.Agency.Remove(agency);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AgencyExists(string id)
        {
            return (_context.Agency?.Any(e => e.Number == id)).GetValueOrDefault();
        }
    }
}
