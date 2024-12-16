using System;

using ActiveLogin.Authentication.BankId.Core.UserContext.Device;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device.Configuration;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace ActiveLogin.Authentication.BankId.Core.Test.UserContext.Device.Configuration;

public class BankIdEndUserDeviceConfigurationBuilder_Tests
{
    private readonly IServiceCollection _services = new ServiceCollection();

    private IBankIdEndUserDeviceConfigurationBuilder CreateSut()
    {
        return new BankIdEndUserDeviceConfigurationBuilder(new BankIdBuilder(_services));
    }

    [Fact]
    public void BankIdEndUserDeviceConfigurationBuilder_DeviceType_DefaultsToWeb()
    {
        // Arrange
        var configurationBuilder = CreateSut();
        // Act
        var deviceType = configurationBuilder.DeviceType;
        // Assert
        Assert.Equal(BankIdEndUserDeviceType.Web, deviceType);
    }

    [Fact]
    public void UseResolverFactory_Sets_ResolverFactory()
    {
        // Arrange
        var configurationBuilder = CreateSut();
        // Act
        configurationBuilder.UseResolverFactory<FakeResolverFactory>();
        // Assert
        var service = _services.BuildServiceProvider().GetService<IBankIdEndUserDeviceDataResolverFactory>();
        Assert.NotNull(service);
    }

    [Fact]
    public void UseResolverFactory_Throws_If_Interface()
    {
        // Arrange
        var configurationBuilder = CreateSut();
        // Act
        var exception = Assert.Throws<ArgumentException>(() => configurationBuilder.UseResolverFactory<IBankIdEndUserDeviceDataResolverFactory>());
        // Assert
        Assert.Equal("T must be a class implementing IBankIdEndUserDeviceDataResolverFactory", exception.Message);
    }

    [Fact]
    public void UseResolver_Adds_Resolver_To_List()
    {
        // Arrange
        var configurationBuilder = CreateSut();
        // Act
        configurationBuilder.UseDeviceResolver<FakeResolver>();
        // Assert
        var service = _services.BuildServiceProvider().GetService<IBankIdEndUserDeviceDataResolver>();
        Assert.NotNull(service);
    }

    [Fact]
    public void UseResolver_Throws_If_Interface()
    {
        // Arrange
        var configurationBuilder = CreateSut();
        // Act
        var exception = Assert.Throws<ArgumentException>(() => configurationBuilder.UseDeviceResolver<IBankIdEndUserDeviceDataResolver>());
        // Assert
        Assert.Equal("T must be a class implementing IBankIdEndUserDeviceDataResolver", exception.Message);
    }


}
