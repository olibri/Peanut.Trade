using System.Text.Json.Serialization;

namespace Domain.Models.JsonDto.Kucoin;

public class KuCoinDataGeneral
{
    [JsonPropertyName("data")]
    public KuCoinTickerData Data { get; set; }
}