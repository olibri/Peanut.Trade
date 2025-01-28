using Domain.Models.Records.ServiceDtos;

namespace Domain.Exception;

public class AppPairNotSupportedException(CurrencyPair pair)
    : System.Exception($"No App support currency pair {pair}")
{
    public CurrencyPair Pair { get; } = pair;
}