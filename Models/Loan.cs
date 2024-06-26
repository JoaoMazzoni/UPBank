using System.ComponentModel.DataAnnotations;

namespace Models;

public class Loan
{
    public int Id { get; set; }
    public string AccountNumber { get; set; }
    public string CustomerDocument { get; set; }
}