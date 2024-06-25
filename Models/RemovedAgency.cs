using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class RemovedAgency
    {
        [Key]
        public string Number { get; set; }
        [NotMapped]
        public Address Address { get; set; }
        public string AddressId { get; set; }
        public string CNPJ { get; set; }
        public List<RemovedAgencyEmployee> Employees { get; set; }
        public bool Restriction { get; set; }
        

        public static string RemoveMask(string cnpj)
        {
            cnpj = cnpj.Replace(".", "");
            cnpj = cnpj.Replace("/", "");
            cnpj = cnpj.Replace("-", "");
            return cnpj;
        }

        public static string InsertMask(string cnpj)
        {
            return Convert.ToUInt64(cnpj).ToString(@"00\.000\.000\/0000\-00");
        }
    }
}