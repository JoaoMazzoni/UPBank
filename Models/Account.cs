using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Models;

public class Account
{
    [Key] public string Number { get; set; }
    public Agency Agency { get; set; }
    public string MainCustomerId { get; set; }
    public Client MainCustomer { get; set; }
    public string? SecundaryCustomerId { get; set; }
    public Client? SecundaryCustomer { get; set; }
    public bool Restriction { get; set; }
    public CreditCard? CreditCard { get; set; }
    public double SpecialLimit { get; set; }
    public DateTime Date { get; set; }
    public double Balance { get; set; }
    public List<Operation> Extract { get; set; }
    public ICollection<OperationAccount> OperationAccounts { get; set; }

    public enum Profile
    {
        Academic,
        Normal,
        Premium
    }
}