using System.Net.Http.Json;
using Domain.Exception;
using Domain.Models.JsonDto.Kucoin;
using Domain.Models.Records.ServiceDtos;

namespace Infrastructure.Services.Exchanges;

public class KucoinService(HttpClient client, HashSet<string> userSupportedCurrencies)
    : BaseExchangeService(userSupportedCurrencies)
{
    private const string ExchangeInfoEndpoint = "api/v2/symbols";
    private const string PriceTickerEndpoint = "/api/v1/market/orderbook/level1";

    public override string Name => "Kucoin";

    protected override async Task<decimal> GetPriceFromApiAsync(CurrencyPair pair)
    {
        var symbol = $"{pair.BaseAsset}-{pair.QuoteAsset}";

        var response = await client.GetAsync($"{PriceTickerEndpoint}?symbol={symbol}");
        response.EnsureSuccessStatusCode();

        var ticker = await response.Content.ReadFromJsonAsync<KuCoinDataGeneral>();
        return ticker?.Data.Price ?? throw new MarketDataUnavailableException(Name, pair);
    }
    protected override async Task<HashSet<CurrencyPair>> LoadSupportedSymbolsFromExchangeAsync()
    {
        var response = await client.GetFromJsonAsync<KuCoinSymbols>(ExchangeInfoEndpoint);
        return response?.Data
            .Select(s => new CurrencyPair(s.BaseAsset, s.QuoteAsset))
            .ToHashSet() ?? new HashSet<CurrencyPair>();
    }
}