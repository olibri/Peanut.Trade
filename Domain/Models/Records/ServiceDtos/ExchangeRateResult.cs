using Domain.Interfaces;

namespace Domain.Models.Records.ServiceDtos;

public record ExchangeRateResult(
    IExchangeService Exchange,
    decimal Rate,
    bool IsSuccess
);