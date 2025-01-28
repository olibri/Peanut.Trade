using Autofac;
using Domain.Interfaces;

namespace Api.Composition;

public static class ExchangeRegistrationExtensions
{
   public static void RegisterExchange<TExchange>(this ContainerBuilder builder, string configSectionKey)
        where TExchange : IExchangeService
    {
        builder.Register(ctx =>
            {
                var configuration = ctx.Resolve<IConfiguration>();

                var section = configuration.GetSection(configSectionKey);

                var baseUrl = section["BaseUrl"];
                var currencyArray = section.GetSection("SupportedCurrencies").Get<string[]>();

                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(baseUrl)
                };

                var userCurrencies = new HashSet<string>(currencyArray);

                return (TExchange)Activator.CreateInstance(typeof(TExchange), httpClient, userCurrencies);

            })
            .As<IExchangeService>()
            .SingleInstance();
    }
}