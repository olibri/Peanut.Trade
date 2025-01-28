using Domain.Exception;
using Domain.Models.Records.ControllerDtos.Request;
using Domain.Models.Records.ControllerDtos.Response;
using Domain.Models.Records.ServiceDtos;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Peanut.Trade.Test.Extensions;

namespace Peanut.Trade.Test;

//Quick simple tests for sure it's working
//for production we need to add more tests for different cases
public class RatesTests(TestFixture fixture) : ExchangeControllerTests(fixture)
{
    [Fact]
    public async Task GetRates_ValidCurrencyPair_ReturnsNonEmptyRatesList()
    {
        var request = new RateRequest(new CurrencyPair("BTC", "USDT"));

        var result = await ExchangeController.GetRates(request);

        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeAssignableTo<IEnumerable<RateResponse>>();

        var rates = okResult.Value as IEnumerable<RateResponse>;
        rates.Should().NotBeEmpty().And.AllSatisfy(rate => {
            rate.Rate.Should().BePositive();
            rate.ExchangeName.Should().NotBeNullOrWhiteSpace();
        });
    }

    [Theory]
    [InlineData("", "btc")]
    [InlineData("BTC", "")]
    public async Task GetRates_InvalidCurrencyPair_ReturnsBadRequest(string input, string output)
    {
        var request = new RateRequest(new CurrencyPair(input, output));

        await FluentActions
            .Invoking(() => ExchangeController.GetRates(request))
            .Should()
            .ThrowAsync<ExchangePairNotSupportedException>();
    }

    [Fact]
    public async Task GetRates_UnsupportedCurrencyPair_ReturnsEmptyRatesList()
    {
        var request = new RateRequest(new CurrencyPair("UNKNOWN", "BTC"));

        await FluentActions
            .Invoking(() => ExchangeController.GetRates(request))
            .Should()
            .ThrowAsync<AppPairNotSupportedException>();
    }
}