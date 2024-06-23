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
    public string MainClientId { get; set; }
    public Client MainClient { get; set; }
    public string? SecundaryClientId { get; set; }
    public Client? SecundaryClient { get; set; }
    public bool Restriction { get; set; }
    public CreditCard? CreditCard { get; set; }
    public double SpecialLimit { get; set; }
    public List<Operation> Statement { get; set; }

    public enum Profile
    {
        Academic,
        Normal,
        Premium
    }

    public DateTime Date { get; set; }
    public double Balance { get; set; }
    public ICollection<OperationAccount> OperationAccounts { get; set; }
}