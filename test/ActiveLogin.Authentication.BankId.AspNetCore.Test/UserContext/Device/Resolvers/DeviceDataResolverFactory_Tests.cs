using System;
using System.Collections.Generic;

using ActiveLogin.Authentication.BankId.AspNetCore.UserContext.Device.Resolvers;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device.Configuration;

using Xunit;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.UserContext.Device.Resolvers;

public class DefaultEndUserDeviceDataResolverFactoryTests
{
    private IBankIdEndUserDeviceDataResolverFactory CreateSut(IBankIdEndUserDeviceDataConfiguration config)
    {
        return new BankIdDefaultEndUserDeviceDataResolverFactory(
            config,
            new List<IBankIdEndUserDeviceDataResolver>
            {
                new FakeResolverApp(),
                new FakeResolverWeb()
            });
    }
    
    [Theory]
    [InlineData(BankIdEndUserDeviceType.App, typeof(FakeResolverApp))]
    [InlineData(BankIdEndUserDeviceType.Web, typeof(FakeResolverWeb))]
    public void GetResolver_Returns_Correct_Resolver_Type(BankIdEndUserDeviceType type, Type expectedType)
    {
        // Arrange
        var sut = CreateSut(new DefaultBankIdEndUserDeviceDataConfiguration()
        {
            DeviceType = type
        });

        // Act
        var resolver = sut.GetResolver();

        // Assert
        Assert.IsType(expectedType, resolver);
    }   

}
