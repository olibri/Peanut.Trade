using System.Net;
using System.Text.Json;
using Domain.Exception;

namespace Api.Composition;

public class GlobalExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var code = HttpStatusCode.InternalServerError;
        object errorObject;

        switch (ex)
        {
            case ExchangePairNotSupportedException pairEx:
                code = HttpStatusCode.NotFound;
                errorObject = new
                {
                    error = pairEx.Message,
                    pair = pairEx.Pair.ToString()
                };
                break;

            case AppPairNotSupportedException appEx:
                code = HttpStatusCode.BadRequest;
                errorObject = new
                {
                    error = appEx.Message,
                    pair = appEx.Pair.ToString()
                };
                break;

            case MarketDataUnavailableException marketEx:
                code = HttpStatusCode.ServiceUnavailable;
                errorObject = new
                {
                    error = marketEx.Message,
                    exchange = marketEx.Name,
                    pair = marketEx.Pair.ToString()
                };
                break;

            case RequestAmountException amtEx:
                code = HttpStatusCode.BadRequest;
                errorObject = new
                {
                    error = amtEx.Message,
                    amount = amtEx.Amount
                };
                break;

            default:
                errorObject = new { error = "Internal server error" };
                break;
        }

        var result = JsonSerializer.Serialize(errorObject);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        await context.Response.WriteAsync(result);
    }
}