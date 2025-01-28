using Domain.Models.Records.ServiceDtos;

namespace Domain.Exception;

public class MarketDataUnavailableException(string name, CurrencyPair pair)
    : System.Exception($"Market Data Unavailable {name} {pair}")
{
    public CurrencyPair Pair { get; } = pair;
    public string Name { get; } = name;
}