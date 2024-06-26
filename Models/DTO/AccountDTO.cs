using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Models.DTO;

public class AccountDTO
{
    public string Number { get; set; }
    public string AgencyNumber { get; set; }
    public string SavingsAccountNumber { get; set; }
    public string MainCustomerId { get; set; }
    public string? SecondaryCustomerId { get; set; }
    public bool Restriction { get; set; }
    public double SpecialLimit { get; set; }
    public DateTime Date { get; set; }
    public double Balance { get; set; }
}