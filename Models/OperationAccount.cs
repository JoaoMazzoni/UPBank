using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

public class OperationAccount
{
    [Key] [Column(Order = 1)] public string AccountId { get; set; }
    public Account Account { get; set; }

    [Key] [Column(Order = 2)] public int OperationId { get; set; }
    public Operation Operation { get; set; }
}