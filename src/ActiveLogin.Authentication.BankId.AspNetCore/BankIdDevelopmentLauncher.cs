using ActiveLogin.Authentication.BankId.AspNetCore.UserMessage;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdDevelopmentLauncher : IBankIdLauncher
    {
        public string GetLaunchUrl(BankIdSupportedDevice device, LaunchUrlRequest request)
        {
            if (device.IsIos)
            {
                return request.RedirectUrl;
            }

            return "#";
        }
    }
}