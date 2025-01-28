namespace Domain.Models.Records.ControllerDtos.Request;

public record EstimateRequest(
    decimal InputAmount,
    string InputCurrency,
    string OutputCurrency
);