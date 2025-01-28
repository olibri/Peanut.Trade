using System.Net.Http.Json;
using Domain.Exception;
using Domain.Models.JsonDto.Binance;
using Domain.Models.Records.ServiceDtos;

namespace Infrastructure.Services.Exchanges;

//For production,
//it's better to use some cached responses
//so you don't have to go after the result every time because it takes a long time.
//This applies to two exchanges!!!
public class BinanceService(HttpClient client, HashSet<string> userSupportedCurrencies)
    : BaseExchangeService(userSupportedCurrencies)
{
    private const string ExchangeInfoEndpoint = "/api/v3/exchangeInfo";
    private const string PriceTickerEndpoint = "/api/v3/ticker/price";
    public override string Name => "Binance";

    protected override async Task<decimal> GetPriceFromApiAsync(CurrencyPair pair)
    {
        var symbol = $"{pair.BaseAsset}{pair.QuoteAsset}";

        var response = await client.GetAsync($"{PriceTickerEndpoint}?symbol={symbol}");
        response.EnsureSuccessStatusCode();
        var ticker = await response.Content.ReadFromJsonAsync<BinancePrice>();

        return ticker?.Price ?? throw new MarketDataUnavailableException(Name, pair);
    }

    protected override async Task<HashSet<CurrencyPair>> LoadSupportedSymbolsFromExchangeAsync()
    {
        var response = await client.GetFromJsonAsync<BinanceExchangeInfo>(ExchangeInfoEndpoint);
        return response?.Symbols
            .Select(s => new CurrencyPair(s.BaseAsset, s.QuoteAsset))
            .ToHashSet() ?? new HashSet<CurrencyPair>();
    }
}