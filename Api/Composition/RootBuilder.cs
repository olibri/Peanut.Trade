using Api.Controllers;
using Application.Services;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Domain.Interfaces;
using Infrastructure.Services.Exchanges;
using Microsoft.OpenApi.Models;


namespace Api.Composition;

public static class RootBuilder
{
    public static IHost GetHost()
    {

        var host = Host.CreateDefaultBuilder()

            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterType<ExchangeAggregator>().As<IExchangeAggregator>();
                builder.RegisterType<ExchangeRegistry>().As<IExchangeRegistry>();

                builder.RegisterExchange<BinanceService>("ExchangeSettings:Binance");
                builder.RegisterExchange<KucoinService>("ExchangeSettings:KuCoin");

                builder.RegisterType<ExchangeController>().InstancePerDependency();

            })

            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.Configure(app =>
                {
                    app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

                    app.UseSwagger();
                    app.UseSwaggerUI();
                    app.UseHttpsRedirection();
                    app.UseRouting();

                    app.UseAuthentication();
                    app.UseAuthorization();

                    app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
                });
            })
            .ConfigureServices(services =>
            {
                services.AddControllers();
                services.AddEndpointsApiExplorer();
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "api/Test", Version = "v1" });

                });

                services.AddHttpClient();
                services.AddAuthorization();
            })
            .Build();

        return host;
    }
}