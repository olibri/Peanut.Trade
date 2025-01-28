using Domain.Interfaces;
using Domain.Models.Records.ControllerDtos.Request;
using Domain.Models.Records.ControllerDtos.Response;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExchangeController(IExchangeAggregator exchangeAggregator) : ControllerBase
{
    [HttpGet("estimate")]
    public async Task<IActionResult> Estimate([FromQuery] EstimateRequest dto)
    {
        var response = await exchangeAggregator.GetBestEstimateAsync(dto);
        var result = new BestEstimateResponse(response.ExchangeName, response.OutputAmount);
        return Ok(result);
    }

    [HttpGet("rates")]
    public async Task<IActionResult> GetRates([FromQuery] RateRequest dto)
    {
        var domainRates = await exchangeAggregator.GetAllRatesAsync(dto.pair);
        var result = domainRates
            .Select(x => new RateResponse(x.ExchangeName, x.Rate))
            .ToList();

        return Ok(result);
    }
}
