using Models;
using Models.DTO;

namespace AccountAPI.Services;

public class OperationService
{
    public Operation GenerateOperation(OperationDTO operationDTO)
    {
        Operation operation = new()
        {
            Id = operationDTO.Id,
            Date = DateTime.Now,
            Type = operationDTO.Type,
            Value = operationDTO.Value
        };

        return operation;
    }
}