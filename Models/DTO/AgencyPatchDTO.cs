using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class AgencyPatchDTO
    {
        public AddressDTO Address { get; set; }
        public List<string> Employees { get; set; }
    }
}
