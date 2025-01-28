using Domain.Models.Records.ServiceDtos;

namespace Domain.Interfaces;

public interface IExchangeService
{
    string Name { get; }
    Task<decimal> GetPriceAsync(CurrencyPair currencyPair);
    Task<bool> SupportsPairAsync(CurrencyPair pair);
}