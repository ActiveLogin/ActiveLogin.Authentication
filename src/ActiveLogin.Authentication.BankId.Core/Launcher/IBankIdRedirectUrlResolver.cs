namespace ActiveLogin.Authentication.BankId.Core.Launcher;

public enum BankIdTransactionType
{
    Auth,
    Sign,
    Payment
}

public interface IBankIdRedirectUrlResolver
{
    Task<BankIdRedirectUrl> GetRedirectUrl(BankIdTransactionType type, string callbackPath);
}
