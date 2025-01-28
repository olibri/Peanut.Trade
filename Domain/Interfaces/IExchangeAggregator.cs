using Domain.Models.Records.ControllerDtos.Request;
using Domain.Models.Records.ControllerDtos.Response;
using Domain.Models.Records.ServiceDtos;

namespace Domain.Interfaces;

public interface IExchangeAggregator
{
    Task<BestEstimateResponse> GetBestEstimateAsync(EstimateRequest request);
    Task<IEnumerable<RateResponse>> GetAllRatesAsync(CurrencyPair pair);
}