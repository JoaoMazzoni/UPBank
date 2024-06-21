using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Models
{
    public class Account
    {
        public Agency Agency { get; set; }
        public string Number { get; set; }
        public List<Client> Costumers { get; set; }
        public bool Restriction { get; set; }   
        public CreditCard CreditCard { get; set; }
        public double SpecialLimit { get; set; }
        public enum Profile { Academic, Normal, Premium }
        public DateTime Date { get; set; }
        public double Balance { get; set; }
        public List<Operation> Extract { get; set;}


    }
}
