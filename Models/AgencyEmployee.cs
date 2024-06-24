using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

public class AgencyEmployee
{
    [Key] [Column(Order = 1)] public string AgencyId { get; set; }
    public Agency Agency { get; set; }

    [Key] [Column(Order = 2)] public string EmployeeId { get; set; }
    public Employee Employee { get; set; }
}