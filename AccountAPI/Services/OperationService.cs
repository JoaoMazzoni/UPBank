using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO;
using SQLitePCL;
using Type = Models.Type;

namespace AccountAPI.Services;

public class OperationService
{
    public Operation GenerateOperation(OperationDTO operationDTO, bool secundaryAccount)
    {
        if (secundaryAccount)
        {
            Operation operation = new()
            {
                Id = 0,
                Date = DateTime.Now,
                Account = new Account
                {
                    Number = operationDTO.TargetAccountNumber
                },
                Type = operationDTO.Type,
                Value = operationDTO.Value
            };
            return operation;
        }
        else
        {
            Operation operation = new()
            {
                Id = 0,
                Date = DateTime.Now,
                Account = null,
                Type = operationDTO.Type,
                Value = operationDTO.Value
            };
            return operation;
        }
    }

    public bool CheckOperation(Account Account, OperationDTO operationDTO)
    {
        if (operationDTO.Value <= 0)
        {
            throw new ArgumentException("Impossivel fazer operação com valores menores ou iguais a 0");
        }

        if (Account.Restriction)
        {
            throw new ArgumentException(
                "A conta possui restrição e não é possivel realizar operações, entre em contato com o gerente");
        }

        if ((int)operationDTO.Type == 3 || (int)operationDTO.Type == 0 || (int)operationDTO.Type == 4)
        {
            if (operationDTO.Value > Account.Balance)
            {
                throw new InvalidOperationException("Não possui saldo suficiente para processar a transação");
            }
        }

        return true;
    }
}