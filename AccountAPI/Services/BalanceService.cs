using Models;
using Models.DTO;

namespace AccountAPI.Services;

public class BalanceService
{
    public BalanceDTO PopulateBalanceDto(Account account)
    {
        return new BalanceDTO()
        {
            AccountNumber = account.Number,
            CurrentBalance = account.Balance,
            ConsultedIn = DateTime.Now
        };
    }
}