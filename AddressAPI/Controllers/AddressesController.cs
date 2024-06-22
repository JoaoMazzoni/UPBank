using AddressAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTO;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using System.Net;


namespace AddressAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly AddressService _addressService;

        public AddressesController(AddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_addressService.GetAll());
        }

        //Arrumar aqui para buscar por Id
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {

            var address = _addressService.Get(id);

            if (address == null)
            {
                return NotFound();
            }

            return Ok(address);
        }

        
        [HttpPost]
        public async Task<ActionResult<Address>> PostAddress([FromBody] AddressDTO address)
        {
            using (var client = new HttpClient())
            {
                string zipCode = address.ZipCode;
                client.BaseAddress = new Uri("https://viacep.com.br/");
                var response = await client.GetAsync($"ws/{zipCode}/json/");

                if (response.IsSuccessStatusCode)
                {
                    var stringResult = await response.Content.ReadAsStringAsync();
                    var add = Newtonsoft.Json.JsonConvert.DeserializeObject<Address>(stringResult);

                    add.Complement = address.Complement;
                    add.Number = address.Number;
                    add.Id = $"{address.ZipCode}{add.Number}";


                    var result = _addressService.Post(add);

                    return add;
                }
                else
                {
                    return BadRequest("Erro ao obter endereço do serviço ViaCEP");
                }
            }
        }

        //private bool AddressExists(string id)
        //{
        //    return _addressService.Get(id) != null;
        //}

        //private Address AddressExists(string id)
        //{
        //    var address = _addressService.Get(id);
        //    if (address == null)
        //    {
        //        return null;
        //    }
        //    return address;
        //}

        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] Address address)
        {
            var addressToUpdate = _addressService.Get(id);

            if (addressToUpdate == null)
            {
                return NotFound();
            }

            _addressService.Update(id, address);

            return NoContent();
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var address = _addressService.Get(id);

            if (address == null)
            {
                return NotFound();
            }

            _addressService.Delete(id);

            return NoContent();
        }


      

    }
}
