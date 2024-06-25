using System.ComponentModel.DataAnnotations;

namespace Models.DTO
{
    public class AgencyDTO
    {
        [Key]
        public string Number { get; set; }
        public AddressDTO Address { get; set; }
        public string CNPJ { get; set; }
        public List<string> Employees { get; set; }
        public bool Restriction { get; set; }
    }
}
