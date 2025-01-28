using Domain.Models.Records.ServiceDtos;

namespace Domain.Interfaces;

public interface IExchangeRegistry
{
     Task<IEnumerable<IExchangeService>> CheckSupportedPairsOnExchange(CurrencyPair pair);
}
