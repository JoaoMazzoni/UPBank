using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CreditCard
    {
        public long Number { get; set; }   
        public DateTime Date { get; set; }
        public double Limit { get; set; }
        public string SecurityCode { get; set; }    
        public string Owner { get; set; } 
        public string Flag { get; set; }    
    }
}
