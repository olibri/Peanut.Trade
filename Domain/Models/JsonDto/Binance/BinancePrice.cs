using System.Text.Json.Serialization;

namespace Domain.Models.JsonDto.Binance;

public class BinancePrice
{

    [JsonPropertyName("price")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal Price { get; set; }
}