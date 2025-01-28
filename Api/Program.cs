using Api.Composition;

var host = RootBuilder.GetHost();
await host.RunAsync();