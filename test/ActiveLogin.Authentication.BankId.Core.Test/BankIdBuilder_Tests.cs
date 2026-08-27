using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace ActiveLogin.Authentication.BankId.Core.Test;
public class BankIdBuilderTests
{
    [Fact]
    public void AddSimulatedApiErrors_Throws_If_Multiple_IBankIdAppApiClient_Exists_In_ServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IBankIdAppApiClient, BankIdAppApiClient>();
        services.AddSingleton<IBankIdAppApiClient, BankIdAppApiClient>();
        var builder = new BankIdBuilder(services);
        var exception = Assert.Throws<InvalidOperationException>(() => builder.AddSimulatedApiErrors());
        Assert.Equal("Multiple IBankIdAppApiClient implementations found in the service collection. Only one implementation is allowed.", exception.Message);
    }

    [Fact]
    public void AddSimulatedApiErrors_Throws_If_No_IBankIdAppApiClient_Exists_In_ServiceCollection()
    {
        var services = new ServiceCollection();
        var builder = new BankIdBuilder(services);
        var exception = Assert.Throws<InvalidOperationException>(() => builder.AddSimulatedApiErrors());
        Assert.Equal("No IBankIdAppApiClient implementation found in the service collection.", exception.Message);

    }

    [Fact]
    public async Task UseSimulatedEnvironment_WithOptions__RegistersConfiguredCollectStates()
    {
        var services = new ServiceCollection();
        var builder = new BankIdBuilder(services);
        var states = new List<BankIdSimulatedAppApiClient.CollectState>
        {
            new(CollectStatus.Pending, CollectHintCode.NoClient),
            new(CollectStatus.Complete, CollectHintCode.UserSign)
        };

        builder.UseSimulatedEnvironment(options => options.CollectStates = states);

        using var serviceProvider = services.BuildServiceProvider();
        var client = Assert.IsType<BankIdSimulatedAppApiClient>(serviceProvider.GetRequiredService<IBankIdAppApiClient>());
        client.Delay = TimeSpan.Zero;
        var authResponse = await client.AuthAsync(new AuthRequest("1.1.1.1"));
        var firstCollectResponse = await client.CollectAsync(new CollectRequest(authResponse.OrderRef));
        var secondCollectResponse = await client.CollectAsync(new CollectRequest(authResponse.OrderRef));

        Assert.Equal(CollectStatus.Pending, firstCollectResponse.GetCollectStatus());
        Assert.Equal(CollectHintCode.NoClient, firstCollectResponse.GetCollectHintCode());
        Assert.Equal(CollectStatus.Complete, secondCollectResponse.GetCollectStatus());
    }

}
