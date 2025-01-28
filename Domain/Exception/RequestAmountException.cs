namespace Domain.Exception;

public class RequestAmountException(decimal amount)
    : System.Exception($"Amount should be great then 0 {amount}")
{
    public decimal Amount{ get; } = amount;
}