namespace ActiveLogin.Authentication.BankId.AspNetCore.UserContext.Device.State;

/// <summary>
/// State for web device data.
/// </summary>
/// <param name="deviceIdentifier"></param>
public class DeviceDataState(string deviceIdentifier)
{
    public string DeviceIdentifier { get; set; } = deviceIdentifier;
}
