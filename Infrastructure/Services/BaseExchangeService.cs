using Domain.Exception;
using Domain.Interfaces;
using Domain.Models.Records.ServiceDtos;

namespace Infrastructure.Services;

public abstract class BaseExchangeService(HashSet<string> userSupportedCurrencies) : IExchangeService
{
    protected HashSet<CurrencyPair> ExchangePairs = new();
    protected abstract Task<HashSet<CurrencyPair>> LoadSupportedSymbolsFromExchangeAsync();
    protected abstract Task<decimal> GetPriceFromApiAsync(CurrencyPair pair);
    public abstract string Name { get; }

    #region private methods

    private bool AreSymbolsLoaded => ExchangePairs?.Count > 0;

    private async Task EnsureSymbolsLoadedAsync()
    {
        if (!AreSymbolsLoaded)
            ExchangePairs = await LoadSupportedSymbolsFromExchangeAsync();
    }

    private (CurrencyPair direct, CurrencyPair reverse) GetCurrencyPairs(CurrencyPair currencyPair)
    {
        var baseUpper = currencyPair.BaseAsset.ToUpper();
        var quoteUpper = currencyPair.QuoteAsset.ToUpper();
        return (
            new CurrencyPair(baseUpper, quoteUpper),
            new CurrencyPair(quoteUpper, baseUpper)
        );
    }

    private async Task<decimal?> TryGetDirectPrice(CurrencyPair direct)
    {
        if (!ExchangePairs.Contains(direct)) return null;
        return await GetPriceFromApiAsync(direct);
    }

    private void ValidateCurrenciesSupported(CurrencyPair currencyPair)
    {
        var requiredCurrencies = new[] { currencyPair.BaseAsset, currencyPair.QuoteAsset };
        if (requiredCurrencies.Any(c => !userSupportedCurrencies.Contains(c)))
            throw
                new AppPairNotSupportedException(
                    currencyPair); 
        //The problem condition
        //does not specify how to handle the case when one exchange has a pair and the other does not,
        //so I just throw an error.
    }

    private async Task<decimal?> TryGetReversedPrice(CurrencyPair reverse)
    {
        if (!ExchangePairs.Contains(reverse)) return null;
        var reversePrice = await GetPriceFromApiAsync(reverse);
        return 1 / reversePrice;
    }

    private async Task<(CurrencyPair direct, CurrencyPair reverse)> InitializeAndGetPairsAsync(
        CurrencyPair currencyPair)
    {
        ValidateCurrenciesSupported(currencyPair);
        var pairs = GetCurrencyPairs(currencyPair);
        await EnsureSymbolsLoadedAsync();
        return pairs;
    }

    #endregion

    public virtual async Task<decimal> GetPriceAsync(CurrencyPair currencyPair)
    {
        var (direct, reverse) = await InitializeAndGetPairsAsync(currencyPair);
        return await TryGetDirectPrice(direct) ??
               await TryGetReversedPrice(reverse) ??
               throw new ExchangePairNotSupportedException(currencyPair);
    }

    public async Task<bool> SupportsPairAsync(CurrencyPair currencyPair)
    {
        var (direct, reverse) = await InitializeAndGetPairsAsync(currencyPair);
        return ExchangePairs.Contains(direct) || ExchangePairs.Contains(reverse);
    }
}