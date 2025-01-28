using System.Text.Json.Serialization;

namespace Domain.Models.JsonDto.Kucoin;

public class KuCoinTickerData
{
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}