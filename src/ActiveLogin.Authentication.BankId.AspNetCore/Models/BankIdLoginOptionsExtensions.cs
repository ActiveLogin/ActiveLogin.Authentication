namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    internal static class BankIdLoginOptionsExtensions
    {
        public static bool IsAutoLogin(this BankIdLoginOptions bankIdLoginOptions)
        {
            return !bankIdLoginOptions.AllowChangingPersonalIdentityNumber ||
                   bankIdLoginOptions.BankIdUseQrCode;
        }
    }
}
