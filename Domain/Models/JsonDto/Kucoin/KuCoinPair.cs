using System.Text.Json.Serialization;

namespace Domain.Models.JsonDto.Kucoin;

public class KuCoinPair
{
    [JsonPropertyName("baseCurrency")]
    public string BaseAsset { get; set; }

    [JsonPropertyName("quoteCurrency")]
    public string QuoteAsset { get; set; }
}