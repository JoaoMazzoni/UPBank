using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CopyClasses
{
    public class AccountRequest : Person
    {
        public bool Restriction { get; set; }
    }
}
