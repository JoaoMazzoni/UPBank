using AddressAPI.Utilis;
using Microsoft.AspNetCore.Mvc;
using Models;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Net.Http;

namespace AddressAPI.Services
{
    public class AddressService
    {
        private readonly IMongoCollection<Address> _address;
        private readonly IMongoCollection<Address> _removed;

        public AddressService(IProjMongoDBAPIDataBaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _address = database.GetCollection<Address>(settings.AddressCollectionName);
        }

        public Address Post(Address address)
        {
            if (AddressExists(address.Id))
            {
                throw new Exception("O endereço informado já existe e portando foi atribuído ao cliente.");
            }
            _address.InsertOne(address);
            return address;
        }


        public List<Address> GetAll() =>
            _address.Find(address => true).ToList();

 
        public Address Get(string id)
        {

            var address = _address.Find<Address>(address => address.Id == id).FirstOrDefault();
            if (address == null)
            {
                return null;
            }
            return address;
        }


        private bool AddressExists(string id)
        {
            var address = _address.Find<Address>(address => address.Id == id).FirstOrDefault();
            return address != null;
        }




    }
}
