using Models;
using Models.DTO;
using SQLitePCL;

namespace AccountAPI.Services;

public class OperationService
{
    public Operation GenerateOperation(OperationDTO operationDTO)
    {
        Operation operation = new()
        {
            Id = 0,
            Date = DateTime.Now,
            Account = new ()
            {
                Number = operationDTO.AccountNumber
            },
            Type = operationDTO.Type,
            Value = operationDTO.Value
        };

        return operation;
    }
}