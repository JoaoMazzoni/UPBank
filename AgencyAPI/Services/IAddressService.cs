using Models;
using Models.DTO;

namespace AgencyAPI.Services
{
    public interface IAddressService
    {
        Task<List<Address>> GetAddresses();

        Task<Address> GetAddress(string zipCode);

        Task<Address> PostAddress(AddressDTO address);

        Task<Address> GetAddressById(string id);

        Task<Address> PutAddress(string zipCode, AddressDTO address);   
    }
}
