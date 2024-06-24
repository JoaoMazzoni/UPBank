using System.ComponentModel.DataAnnotations;

namespace Models;

public class DisabledAccount
{
    [Key] public string Number { get; set; }
    public string SavingsAccountNumber { get; set; }
    public string AgencyNumber { get; set; }
    public string MainClientId { get; set; }
    public string? SecondaryClientId { get; set; }
    public bool Restriction { get; set; }
    public CreditCard? CreditCard { get; set; }
    public double SpecialLimit { get; set; }
    public AccountProfile Profile { get; set; }
    public DateTime Date { get; set; }
    public double Balance { get; set; }
}