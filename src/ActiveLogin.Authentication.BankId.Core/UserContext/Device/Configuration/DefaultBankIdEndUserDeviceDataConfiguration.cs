namespace ActiveLogin.Authentication.BankId.Core.UserContext.Device.Configuration;
public class DefaultBankIdEndUserDeviceDataConfiguration : IBankIdEndUserDeviceDataConfiguration
{
    public BankIdEndUserDeviceType DeviceType { get; set; } = BankIdEndUserDeviceType.Web;
    
}
