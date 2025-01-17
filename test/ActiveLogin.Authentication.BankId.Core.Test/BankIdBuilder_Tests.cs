using System;

using ActiveLogin.Authentication.BankId.Api;

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

}
