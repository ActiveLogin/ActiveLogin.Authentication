namespace ActiveLogin.Authentication.BankId.Core.UserContext.Device;

/// <summary>
/// Factory for fetching the right resolver for the current device data configuration.
/// </summary>
public interface IBankIdEndUserDeviceDataResolverFactory
{
    /// <summary>
    /// Get the resolver for the current device data configuration.
    /// </summary>
    /// <returns></returns>
    IBankIdEndUserDeviceDataResolver GetResolver();
}
