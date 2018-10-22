using ActiveLogin.Authentication.BankId.AspNetCore.UserMessage;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public interface IBankIdLauncher
    {
        string GetLaunchUrl(BankIdSupportedDevice device, LaunchUrlRequest request);
    }
}