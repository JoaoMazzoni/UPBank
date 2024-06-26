namespace Models.DTO;

public class AccountInsertDTO
{
    public string Number { get; set; }
    public string AgencyNumber { get; set; }
    public string SavingsAccountNumber { get; set; }
    public string MainCustomerId { get; set; }
    public string? SecondaryCustomerId { get; set; }
}