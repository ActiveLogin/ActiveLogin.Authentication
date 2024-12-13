using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Core.UserContext.Device;

/// <summary>
/// Parameters for the device where the BankID app is running.
/// </summary>
public interface IBankIdEndUserDeviceDataConfiguration
{
    /// <summary>
    /// The type of device that BankId client is launched from.
    /// </summary>
    BankIdEndUserDeviceType DeviceType { get; set; }

    ///// <summary>
    ///// The resolver for the device parameters.
    ///// </summary>
    //Type DeviceDataResolverFactory { get; set; }

}
