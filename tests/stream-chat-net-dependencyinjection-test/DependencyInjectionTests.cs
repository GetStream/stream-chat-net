using Xunit;
using Microsoft.Extensions.DependencyInjection;
using StreamChat;

public class DependencyInjectionTests
{
    [Fact]
    public void TestServiceCollectionReturnsClient()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddStreamClient("apiKey", "apiSecret");
        var provider = serviceCollection.BuildServiceProvider();

        var client = provider.GetRequiredService<IClient>();

        Assert.NotNull(client);
    }

    [Fact]
    public void TestServiceCollectionReturnsClientWithClientOpts()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddStreamClient("apiKey", "apiSecret", new ClientOptions { Timeout = 1000 });
        var provider = serviceCollection.BuildServiceProvider();

        var client = provider.GetRequiredService<IClient>();

        Assert.NotNull(client);
    }
}