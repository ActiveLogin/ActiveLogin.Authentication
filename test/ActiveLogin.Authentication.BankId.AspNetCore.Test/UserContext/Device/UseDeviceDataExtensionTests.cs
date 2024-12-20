using ActiveLogin.Authentication.BankId.AspNetCore.Auth;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.UserContext.Device;

public class UseDeviceDataExtensionTests
{

    [Fact]
    public void CanResolve_DefaultDeviceDataServices()
    {
        var services = new ServiceCollection();
        services.AddAuthentication()
            .AddBankIdAuth(_ => { });
        services.AddLogging();
        services.AddBankId(bankId => { bankId.UseSimulatedEnvironment(); });

        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<IBankIdEndUserDeviceDataResolverFactory>());
        Assert.NotNull(provider.GetService<IBankIdEndUserDeviceDataConfiguration>());
        Assert.NotNull(provider.GetService<IBankIdEndUserDeviceDataResolver>());
        Assert.NotNull(provider.GetService<IBankIdDeviceDataProtector>());

    }

}
