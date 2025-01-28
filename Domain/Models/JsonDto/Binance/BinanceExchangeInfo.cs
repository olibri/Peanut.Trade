using System.Text.Json.Serialization;

namespace Domain.Models.JsonDto.Binance;

public class BinanceExchangeInfo
{
    [JsonPropertyName("symbols")]
    public List<BinancePair> Symbols { get; set; }
}