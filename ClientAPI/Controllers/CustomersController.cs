﻿
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CustomerAPI.Data;
using Models;
using Models.DTO;
using Models.Utils;

namespace CustomerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerAPIContext _context;
        private readonly AddressService _addressService;

        public CustomersController(CustomerAPIContext context, AddressService addressService)
        {
            _context = context;
            _addressService = addressService;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomer()
        {
            if (_context.Customer == null)
            {
                return NotFound();
            }
            var customers =  await _context.Customer.ToListAsync();

            foreach(var customer in customers)
            {
                if(customer.AddressId != null)
                {
                    customer.Address = await _addressService.GetAddressByAPI(customer.AddressId);
                }
            }

            return customers;
        }

        // GET: api/Customers/5
        [HttpGet("{document}")]
        public async Task<ActionResult<Customer>> GetCustomer(string document)
        {
            if (_context.Customer == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer.FindAsync(document);

            if (customer == null)
            {
                return NotFound();
            }

            if(customer.AddressId != null)
            {
                customer.Address = await _addressService.GetAddressByAPI(customer.AddressId);
            }

            return customer;
        }


        [HttpGet("byAddressId")]
        public async Task<ActionResult<List<Customer>>> GetCustomersByAddressId([FromQuery] string AddressId)
        {
            var customers = await _context.Customer.Where(customer => customer.AddressId == AddressId).ToListAsync();
            if (customers == null || customers.Count == 0)
            {
                return NotFound("Nenhum cliente encontrado com o AddressId fornecido.");
            }
            return Ok(customers);
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{document}")]
        public async Task<IActionResult> PutCustomer(string document, Customer customer)
        {
            if (document != customer.Document)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(document))
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

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(CustomerDTO customerDTO)
        {
            var customer = new Customer(customerDTO);

            var address = await _addressService.GetAddressByAPI(customerDTO.AddressDTO.ZipCode + customerDTO.AddressDTO.Number);

            if (address == null)
            {
                Address add = await _addressService.PostAddress(customerDTO.AddressDTO);
                customer.Address = add;
            }
            else
            {
                customer.Address = address;
            }

            if (_context.Customer == null)
            {
                return Problem("Entity set 'CustomerAPIContext.Customer' is null.");
            }

            _context.Customer.Add(customer);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CustomerExists(customer.Document))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCustomer", new { id = customer.Document }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{document}")]
        public async Task<IActionResult> DeleteCustomer(string document)
        {
            
            var customer = await _context.Customer.FindAsync(document);

            if(RemovedCustomerExists(document))
            {
                return Conflict("Este cliente já está removido!");
            }

            else if (customer == null)
            {
                return Conflict("Cliente não encontrado cadastro. Verifique as credenciais e tente novamente.");
            }

            try
            {
                var copyCustomer = CopyCustomer(customer);
                _context.Customer.Remove(customer);
                _context.RemovedCustomer.Add(copyCustomer);

                await _context.SaveChangesAsync();

                if(!CustomerExists(document))
                {
                    return Ok("Cliente removido com sucesso!");
                }
                return NoContent();
            }
            catch (Exception e)
            {

                return Problem($"Cliente não removido \n {e.Message}"); 
                
            }
            
        }

        private RemovedCustomer CopyCustomer(Customer customer)
        {
            var removedCustomer = new RemovedCustomer
            {

                Document = customer.Document,
                Name = customer.Name,
                BirthDate = customer.BirthDate,
                Gender = customer.Gender,
                Salary = customer.Salary,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                AddressId = customer.AddressId
            };

            return removedCustomer;

        }

        private bool CustomerExists(string document)
        {
            return (_context.Customer?.Any(e => e.Document == document)).GetValueOrDefault();
        }

        private bool RemovedCustomerExists(string document)
        {
            return (_context.RemovedCustomer?.Any(e => e.Document == document)).GetValueOrDefault();
        }
    }
}
