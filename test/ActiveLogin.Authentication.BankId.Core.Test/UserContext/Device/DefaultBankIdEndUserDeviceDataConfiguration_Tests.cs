using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Core.UserContext.Device;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device.Configuration;

using Xunit;

namespace ActiveLogin.Authentication.BankId.Core.Test.UserContext.Device;
public class DefaultBankIdEndUserDeviceDataConfiguration_Tests
{
    [Fact]
    public void DefaultBankIdEndUserDeviceDataConfiguration_DeviceType_DefaultsToWeb()
    {
        // Arrange
        var configuration = new DefaultBankIdEndUserDeviceDataConfiguration();
        // Act
        var deviceType = configuration.DeviceType;
        // Assert
        Assert.Equal(BankIdEndUserDeviceType.Web, deviceType);
    }
}
