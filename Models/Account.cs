using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Models.DTO;

namespace Models;

public class Account
{
    [Key] public string Number { get; set; }
    public string SavingsAccountNumber { get; set; }
    public string AgencyNumber { get; set; }
    public string MainCustomerId { get; set; }
    public string? SecondaryCustomerId { get; set; }
    public bool Restriction { get; set; }
    public CreditCard? CreditCard { get; set; }
    public double SpecialLimit { get; set; }
    public List<Operation> Statement { get; set; }
    public AccountProfile Profile { get; set; }
    public DateTime Date { get; set; }
    public double Balance { get; set; }
    public ICollection<OperationAccount> OperationAccounts { get; set; }

    public Account()
    {
    }

    public Account(AccountInsertDTO dto)
    {
        Number = dto.Number;
        AgencyNumber = dto.AgencyNumber;
        SavingsAccountNumber = dto.SavingsAccountNumber;
        MainCustomerId = dto.MainCustomerId;
        SecondaryCustomerId = dto.SecondaryCustomerId;
        Date = DateTime.Now;
        Restriction = true;
        Balance = 0;
    }

    public Account(AccountDTO dto)
    {
        Number = dto.Number;
        AgencyNumber = dto.AgencyNumber;
        SavingsAccountNumber = dto.SavingsAccountNumber;
        MainCustomerId = dto.MainCustomerId;
        SecondaryCustomerId = dto.SecondaryCustomerId;
        Restriction = dto.Restriction;
        SpecialLimit = dto.SpecialLimit;
        Date = dto.Date;
        Balance = dto.Balance;
    }

    public Account(DisabledAccount disabledAccount)
    {
        Number = disabledAccount.Number;
        AgencyNumber = disabledAccount.AgencyNumber;
        SavingsAccountNumber = disabledAccount.SavingsAccountNumber;
        MainCustomerId = disabledAccount.MainCustomerId;
        SecondaryCustomerId = disabledAccount.SecondaryCustomerId;
        CreditCard = disabledAccount.CreditCard;
        Restriction = disabledAccount.Restriction;
        SpecialLimit = disabledAccount.SpecialLimit;
        Date = disabledAccount.Date;
        Balance = disabledAccount.Balance;
        Profile = disabledAccount.Profile;
    }
}