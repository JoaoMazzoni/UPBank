using System.ComponentModel.DataAnnotations;

namespace Models.DTO;

public class AccountDTO
{
    public string AgencyNumber { get; set; }
    public string Number { get; set; }
    public string MainClientId { get; set; }
    public string? SecundaryClientId { get; set; }
    public bool Restriction { get; set; }
    public CreditCard? CreditCard { get; set; }
    public double SpecialLimit { get; set; }

    public enum Profile
    {
        Academic,
        Normal,
        Premium
    }

    public DateTime Date { get; set; }
    public double Balance { get; set; }
}