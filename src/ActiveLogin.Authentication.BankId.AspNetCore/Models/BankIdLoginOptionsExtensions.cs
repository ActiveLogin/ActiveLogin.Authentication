namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public static class BankIdLoginOptionsExtensions
    {
        public static bool IsAutoLogin(this BankIdLoginOptions bankIdLoginOptions)
        {
            return !bankIdLoginOptions.AllowChangingPersonalIdentityNumber;
        }
    }
}