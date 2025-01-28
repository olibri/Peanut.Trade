using Domain.Interfaces;
using Domain.Models.Records.ServiceDtos;

namespace Application.Services;

public class ExchangeRegistry(IEnumerable<IExchangeService> exchanges) : IExchangeRegistry
{
    public async Task<IEnumerable<IExchangeService>> CheckSupportedPairsOnExchange(
        CurrencyPair pair)
    {
        var result = new List<IExchangeService>();
        foreach (var exchange in exchanges)
        {
            if (await exchange.SupportsPairAsync(pair))
                result.Add(exchange);
        }
        return result;
    }
}