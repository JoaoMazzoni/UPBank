using System.ComponentModel.DataAnnotations;

namespace Models.DTO;

public class AccountDTO
{
    public string Number { get; set; }
    public string AgencyNumber { get; set; }
    public string MainClientId { get; set; }
    public string? SecundaryClientId { get; set; }
    public bool Restriction { get; set; }
    public double SpecialLimit { get; set; }
    public AccountProfile Profile { get; set; }
    public DateTime Date { get; set; }
    public double Balance { get; set; }
}