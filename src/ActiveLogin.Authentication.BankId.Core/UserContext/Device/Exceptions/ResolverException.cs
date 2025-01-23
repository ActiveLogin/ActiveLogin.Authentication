namespace ActiveLogin.Authentication.BankId.Core.UserContext.Device.Exceptions;
public class BankIdDeviceDataResolverException : Exception
{
    public BankIdDeviceDataResolverException(BankIdEndUserDeviceType type) : base($"Could not find IBankIdEndUserDeviceDataResolver for device type {type}")
    { }
    public BankIdDeviceDataResolverException(string message) : base(message)
    { }
}
