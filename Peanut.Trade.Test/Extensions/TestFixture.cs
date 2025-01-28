using Api.Composition;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Peanut.Trade.Test.Extensions;

public class TestFixture : IDisposable
{
    public readonly IHost Host = RootBuilder.GetHost();

    public TestFixture()
    {
        Host.Start();
    }

    public T GetService<T>()
    {
        return Host.Services.GetService<T>()!;
    }

    public void Dispose()
    {
        Host.StopAsync().GetAwaiter().GetResult();
        Host.Dispose();
    }
}