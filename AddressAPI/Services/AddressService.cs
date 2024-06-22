using AddressAPI.Utilis;
using Microsoft.AspNetCore.Mvc;
using Models;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace AddressAPI.Services
{
    public class AddressService
    {
        private readonly IMongoCollection<Address> _address;
        private readonly IMongoCollection <Address> _removed;

        public AddressService(IProjMongoDBAPIDataBaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _address = database.GetCollection<Address>(settings.AddressCollectionName);
            _removed = database.GetCollection<Address>(settings.RemovedAddressCollectionName);

        }


        public Address Post(Address address)
        {
            _address.InsertOne(address);
            return address;
        }

        public List<Address> GetAll() =>
            _address.Find(address => true).ToList();


        public Address Get(string id) => 
            _address.Find<Address>(address => address.Id == id).FirstOrDefault();


        public void Update(string id, Address addressIn) =>
            _address.ReplaceOne(address => address.ZipCode == id, addressIn);


        public Address Delete (string id)
        {
            var address = Get(id);
            if (address != null)
            {
                _address.DeleteOne(address => address.ZipCode == id);
                _removed.InsertOne(address);
            }
            return address;
        }

        
    }
}
