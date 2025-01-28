using Api.Controllers;
using Domain.Exception;
using Domain.Models.Records.ControllerDtos.Request;
using Domain.Models.Records.ControllerDtos.Response;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Peanut.Trade.Test.Extensions;

namespace Peanut.Trade.Test;

//Quick simple tests for sure it's working
//for production we need to add more tests for different cases
public class ExchangeControllerTests (TestFixture fixture) : IClassFixture<TestFixture>
{
    public ExchangeController ExchangeController = fixture.GetService<ExchangeController>();
    
    [Fact]
    public async Task Estimate_ValidRequest_ReturnsBestEstimateWithPositiveAmount()
    {
        var estimateRequest = new EstimateRequest
        (
            20000,
            "USDT",
            "BTC"
        );

        var result = await ExchangeController.Estimate(estimateRequest);
        

        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<BestEstimateResponse>();

        var response = okResult.Value as BestEstimateResponse;
        response!.ExchangeName.Should().NotBeNullOrWhiteSpace();
        response.OutputAmount.Should().BePositive();
    }    
    [Fact]
    public async Task Estimate_EmptyInputCurrency_ReturnsBadRequestWithErrorDetails()
    {
        var request = new EstimateRequest(20000, "", "BTC");

        await FluentActions
            .Invoking(() => ExchangeController.Estimate(request))
            .Should()
            .ThrowAsync<ExchangePairNotSupportedException>();
    }

    [Fact]
    public async Task Estimate_UnsupportedCurrencyPair_ReturnsNotFoundWithPairDetails()
    {
        var request = new EstimateRequest(100, "UNKNOWN", "BTC");

        await FluentActions
            .Invoking(() => ExchangeController.Estimate(request))
            .Should()
            .ThrowAsync<AppPairNotSupportedException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public async Task Estimate_InvalidInputAmount_ReturnsBadRequest(decimal amount)
    {
        var request = new EstimateRequest(amount, "USDT", "BTC");

        await FluentActions
            .Invoking(() => ExchangeController.Estimate(request))
            .Should()
            .ThrowAsync<RequestAmountException>();
    }
}