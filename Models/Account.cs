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
    public string AgencyNumber { get; set; }
    public string MainClientId { get; set; }
    public string? SecondaryClientId { get; set; }
    public bool Restriction { get; set; }
    public CreditCard? CreditCard { get; set; }
    public double SpecialLimit { get; set; }
    public string SavingsAccount { get; set; }
    public List<Operation> Statement { get; set; }
    public AccountProfile Profile { get; set; }
    public DateTime Date { get; set; }
    public double Balance { get; set; }
    public ICollection<OperationAccount> OperationAccounts { get; set; }
}