using Newtonsoft.Json;

namespace Domain.Models.JsonDto.Binance;

public class BinancePair
{
    [JsonProperty("baseAsset")]
    public string BaseAsset { get; set; }

    [JsonProperty("quoteAsset")]
    public string QuoteAsset { get; set; }
}