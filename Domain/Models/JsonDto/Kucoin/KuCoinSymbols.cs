using System.Text.Json.Serialization;

namespace Domain.Models.JsonDto.Kucoin;

public class KuCoinSymbols
{
    [JsonPropertyName("data")]
    public List<KuCoinPair> Data { get; set; }
}