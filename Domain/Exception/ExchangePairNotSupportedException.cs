using Domain.Models.Records.ServiceDtos;

namespace Domain.Exception;

public class ExchangePairNotSupportedException(CurrencyPair pair)
    : System.Exception($"No exchanges support currency pair {pair}")
{
    public CurrencyPair Pair { get; } = pair;
}