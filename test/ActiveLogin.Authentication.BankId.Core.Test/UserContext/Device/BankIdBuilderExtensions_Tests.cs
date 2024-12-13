using System;
using System.Linq;
using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device.Configuration;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using Xunit;

namespace ActiveLogin.Authentication.BankId.Core.Test.UserContext.Device;
public class BankIdBuilderExtensions_Tests
{
    private readonly IServiceCollection _serviceCollectionMock = new ServiceCollection();
    private IBankIdBuilder CreateSut(Type resolverFactory = null, params Type[] resolvers)
    {
        if (resolverFactory != null)
        {
            _serviceCollectionMock.AddTransient(typeof(IBankIdEndUserDeviceDataResolverFactory), resolverFactory);
        }

        if (resolvers.Length > 0)
        {
            foreach (var resolver in resolvers)
            {
                _serviceCollectionMock.AddScoped(typeof(IBankIdEndUserDeviceDataResolver), resolver);
            }
        }


        return new BankIdBuilder(_serviceCollectionMock);
    }

    [Fact]
    public void Builder_Without_Config_Has_No_Services()
    {
        // Arrange
        var bankIdBuilder = CreateSut();
        // Act
        var services = bankIdBuilder.UseDeviceData(config =>
        {

        }).Services.BuildServiceProvider();
        // Assert
        Assert.Empty(services.GetServices(typeof(object)));
    }

    [Fact]
    public void Builder_With_ResolverFactory_Set_Replaces_Old_Resolver()
    {
        // Arrange
        var bankIdBuilder = CreateSut(typeof(FakeResolverFactory));

        // Act
        var services = bankIdBuilder.UseDeviceData(config =>
        {
            config.UseResolverFactory<OtherFakeResolverFactory>();
        }).Services.BuildServiceProvider();

        // Assert
        var factory = services.GetService<IBankIdEndUserDeviceDataResolverFactory>();
        Assert.IsType<OtherFakeResolverFactory>(factory);
    }

    [Fact]
    public void Builder_With_Resolvers_Set_Replaces_Old_Resolvers()
    {
        // Arrange
        var bankIdBuilder = CreateSut(typeof(FakeResolverFactory), typeof(FakeResolver));

        // Act
        var services = bankIdBuilder.UseDeviceData(config =>
        {
            config.AddDeviceResolver<FakeResolverOne>();
            config.AddDeviceResolver<FakeResolverTwo>();
        }).Services.BuildServiceProvider();

        // Assert
        var resolvers = services.GetServices<IBankIdEndUserDeviceDataResolver>().ToList();

        Assert.Equal(2, resolvers.Count);
        Assert.IsType<FakeResolverOne>(resolvers[0]);
        Assert.IsType<FakeResolverTwo>(resolvers[1]);
    }

}

public class OtherFakeResolverFactory : IBankIdEndUserDeviceDataResolverFactory
{
    public IBankIdEndUserDeviceDataResolver GetResolver()
    {
        throw new System.NotImplementedException();
    }
}

public class FakeResolverOne : FakeResolverBase
{ }
public class FakeResolverTwo : FakeResolverBase
{ }

public class FakeResolverBase : IBankIdEndUserDeviceDataResolver
{
    public BankIdEndUserDeviceType DeviceType { get; }
    public Task<IBankIdEndUserDeviceData> GetDeviceDataAsync()
    {
        throw new System.NotImplementedException();
    }

    public IBankIdEndUserDeviceData GetDeviceData()
    {
        throw new System.NotImplementedException();
    }
}
