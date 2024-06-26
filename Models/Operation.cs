using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models;

public class Operation
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public Type Type { get; set; }
    public Account Account { get; set; }
    public double Value { get; set; }
    [JsonIgnore] public ICollection<OperationAccount> OperationAccounts { get; set; }
}