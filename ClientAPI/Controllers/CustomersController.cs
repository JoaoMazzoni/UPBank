using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CustomerAPI.Data;
using Models;
using Models.DTO;
using Models.Utils;
using Models.CopyClasses;

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

            document = FormatCPF(document);

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
        public async Task<IActionResult> PutCustomer(string document, CustomerDTO customerDTO)
        {
            var customer = new Customer(customerDTO);

            var address = await _addressService.GetAddressByAPI(customerDTO.AddressDTO.ZipCode + customerDTO.AddressDTO.Number);

            document = FormatCPF(document);
            customer.Email = customer.Email.ToLower();

            var cpf = FormatCPF(customerDTO.Document);
            customer.Document = cpf;

            customer.AddressId = customer.AddressId.Replace("-", "");

            if (address == null)
            {
                if (customerDTO.AddressDTO.Number <= 0)
                {
                    return BadRequest("Número de endereço inválido.");
                }

                Address add = await _addressService.PostAddress(customerDTO.AddressDTO);
                customer.Address = add;
            }
            else
            {
                customer.Address = address;
            }

            if (document != customer.Document)
            {
                return BadRequest("O documento fornecido não corresponde ao documento do cliente.");
            }

            var existingCustomer = await _context.Customer.AsNoTracking().FirstOrDefaultAsync(c => c.Document == document);

            if (existingCustomer == null)
            {
                return NotFound("Cliente não encontrado.");
            }
            if (customer.Address.Id == null)
            {
                return BadRequest("Endereço inválido.");
            }
            try
            {
                if (existingCustomer.Restriction)
                {
                    if (existingCustomer.Restriction != customer.Restriction)
                    {
                        existingCustomer.Restriction = customer.Restriction;
                        _context.Entry(existingCustomer).Property(c => c.Restriction).IsModified = true;
                        await _context.SaveChangesAsync();
                        return Ok("A restrição do cliente foi atualizada com sucesso.");
                    }
                    else
                    {
                        return BadRequest("Clientes com restrição não podem ter outros dados modificados.");
                    }
                }
                else
                {
                    _context.Entry(customer).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return Ok("Cliente atualizado com sucesso.");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(document))
                {
                    return NotFound("Cliente não encontrado durante a atualização.");
                }
                else
                {
                    return StatusCode(500, "Erro de concorrência ao atualizar o cliente. Tente novamente.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar o cliente: {ex.Message}");
            }
           
        }


        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AccountRequest>> PostCustomer(AccountRequestDTO customerDTO)
        {
            
            var customer = new AccountRequest(customerDTO);

            var address = await _addressService.GetAddressByAPI(customerDTO.AddressDTO.ZipCode + customerDTO.AddressDTO.Number);

            customer.Email = customer.Email.ToLower();

            if (!ValidateCPF(customerDTO.Document))
            {
                return BadRequest("CPF inválido.");
            }

            var cpf = FormatCPF(customerDTO.Document);
            customer.Document = cpf;

            customer.AddressId = customer.AddressId.Replace("-", "");


            if (address == null)
            {
                if (customerDTO.AddressDTO.Number <= 0)
                {
                    return BadRequest("Número de endereço inválido.");
                }

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

            if(CustomerExists(cpf))
            {
                return Conflict("O cliente informado já possui uma conta.");
            }

            if(AccountRequestExists(cpf))
            {
                return Conflict("O cliente informado já possui uma solicitação de conta.");
            }

            if(RemovedCustomerExists(cpf))
            {
                var accountRequest  = ToAccountRequest(await _context.RemovedCustomer.FindAsync(cpf));
                _context.AccountRequest.Add(accountRequest);
                _context.RemovedCustomer.Remove(await _context.RemovedCustomer.FindAsync(cpf));
                await _context.SaveChangesAsync();
                return Ok("O cliente estava removido e foi recuperado para solicitar uma nova conta.");
            }
          
            if(customer.Address.Id == null)
            {
                return BadRequest("Endereço inválido.");
            }
            else
            {
                _context.AccountRequest.Add(customer);
            }

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

            return customer;
        }

        // DELETE: api/Customers/5
        [HttpDelete("{document}")]
        public async Task<IActionResult> DeleteCustomer(string document)
        {
            document = FormatCPF(document);

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

        [HttpPatch("{document}")]
        public async Task<IActionResult> MoveCustomer(string document)
        {
            document = FormatCPF(document);

            var accountRequest = await _context.AccountRequest.FindAsync(document);

            if (accountRequest == null)
            {
                return NotFound("Cliente não encontrado.");
            }

            var customer = MoveAccountRequest(accountRequest);

            _context.Customer.Add(customer);
            _context.AccountRequest.Remove(accountRequest);

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Cliente criado com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar o cliente: {ex.Message}");
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

       private Customer MoveAccountRequest(AccountRequest accountRequest)
        {
            var customer = new Customer
            {
                Document = accountRequest.Document,
                Name = accountRequest.Name,
                BirthDate = accountRequest.BirthDate,
                Gender = accountRequest.Gender,
                Salary = accountRequest.Salary,
                Email = accountRequest.Email,
                Phone = accountRequest.Phone,
                Address = accountRequest.Address,
                AddressId = accountRequest.AddressId
            };
            
            return customer;
        }

        private AccountRequest ToAccountRequest (RemovedCustomer removed)
        {
            var accountRequest = new AccountRequest
            {
                Document = removed.Document,
                Name = removed.Name,
                BirthDate = removed.BirthDate,
                Gender = removed.Gender,
                Salary = removed.Salary,
                Email = removed.Email,
                Phone = removed.Phone,
                Address = removed.Address,
                AddressId = removed.AddressId
            };

            return accountRequest;
        }

        private bool CustomerExists(string document)
        {
            return (_context.Customer?.Any(e => e.Document == document)).GetValueOrDefault();
        }

        private bool RemovedCustomerExists(string document)
        {
            return (_context.RemovedCustomer?.Any(e => e.Document == document)).GetValueOrDefault();
        }

        private bool AccountRequestExists(string document)
        {
            return (_context.AccountRequest?.Any(e => e.Document == document)).GetValueOrDefault();
        }
     
        private bool ValidateCPF(string cpf)
        {
            return Models.Utils.CPFValidator.IsValid(cpf);
        }

        private string FormatCPF(string cpf)
        {
            return Models.Utils.CPFValidator.FormatCPF(cpf);
        }
    }
}



