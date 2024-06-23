using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models;

public class Operation
{
    public int Id { get; set; }
    public DateTime Date { get; set; }

    public enum Type
    {
        Withdraw,
        Deposit,
        Transfer,
        Loan,
        Payment
    }

    public Account? Account { get; set; }
    public double Value { get; set; }

    public ICollection<OperationAccount> OperationAccounts { get; set; }
}