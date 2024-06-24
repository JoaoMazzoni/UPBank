using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Models.DTO;

public class AccountDTO
{
    public string Number { get; set; }
    public string AgencyNumber { get; set; }
    public string SavingsAccountNumber { get; set; }
    public string MainClientId { get; set; }
    public string? SecondaryClientId { get; set; }
    public bool Restriction { get; set; }
    public double SpecialLimit { get; set; }
    public DateTime Date { get; set; }
    public double Balance { get; set; }

    /*
     {
        "number" : "123",
        "agencyNumber": "0001",
        "mainClient" : "Enzo Henrico Leal",
        "secondaryClient" : null,
        "restriction" : true,
        "especialLimit" : 1000,
        "accountProfile" : 1,
        "date" : "2024-06-24 00:00:00"
        "balance" : 0
     }
     *
     */
}