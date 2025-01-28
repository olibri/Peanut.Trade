using Domain.Exception;
using Domain.Interfaces;
using Domain.Models.Records.ControllerDtos.Request;
using Domain.Models.Records.ControllerDtos.Response;
using Domain.Models.Records.ServiceDtos;

namespace Application.Services;

public class ExchangeAggregator(IExchangeRegistry exchangeRegistry) : IExchangeAggregator
{
    #region private 
    //It will better to use general class for response avoiding void methods and throw, but for test task it's ok
    private void ValidateBestRate(ExchangeRateResult bestRate, CurrencyPair pair)
    {
        if (bestRate == null)
            throw new ExchangePairNotSupportedException(pair);
    }
    private void ValidateCurrencies(CurrencyPair pair)
    {
        if (string.IsNullOrWhiteSpace(pair.BaseAsset) ||
            string.IsNullOrWhiteSpace(pair.QuoteAsset))
        {
            throw new ExchangePairNotSupportedException(pair);
        }
    }
    private void ValidateInputAmount(EstimateRequest request)
    {
        if (request.InputAmount <= 0)
            throw new RequestAmountException(request.InputAmount);
    }

    private async Task<List<ExchangeRateResult>> GetRatesAsync(
        IEnumerable<IExchangeService> exchanges,
        CurrencyPair pair)
    {
        var rateTasks = exchanges.Select(async exchange =>
        {
            try
            {
                var rate = await exchange.GetPriceAsync(pair);
                return rate > 0
                    ? new ExchangeRateResult(exchange, rate, true)
                    : new ExchangeRateResult(exchange, rate, false);
            }
            catch
            {
                return new ExchangeRateResult(exchange, 0, false);
            }
        });

        var results = await Task.WhenAll(rateTasks);
        return results.Where(r => r.IsSuccess).ToList();
    }


    #endregion
    public async Task<BestEstimateResponse> GetBestEstimateAsync(EstimateRequest request)
    {
        ValidateInputAmount(request);
        var pair = new CurrencyPair(request.InputCurrency, request.OutputCurrency);
        ValidateCurrencies(pair);
        var exchanges = await exchangeRegistry.CheckSupportedPairsOnExchange(pair);
        var rates = await GetRatesAsync(exchanges, pair);
        var bestRate = rates.MaxBy(x => x.Rate);
        ValidateBestRate(bestRate, pair);

        return new BestEstimateResponse(bestRate.Exchange.Name, request.InputAmount * bestRate.Rate);
    }

    public async Task<IEnumerable<RateResponse>> GetAllRatesAsync(CurrencyPair pair)
    {
        ValidateCurrencies(pair);
        var exchanges = await exchangeRegistry.CheckSupportedPairsOnExchange(pair);
        var rates = await GetRatesAsync(exchanges, pair);
        return rates.Select(r => new RateResponse(r.Exchange.Name, r.Rate));
    }
}