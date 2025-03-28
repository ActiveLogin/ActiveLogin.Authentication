namespace ActiveLogin.Authentication.BankId.AspNetCore.Payment;

public interface IBankIdPaymentService
{
    Task InitiatePaymentAsync(BankIdPaymentProperties properties, string callbackPath, string configKey);
    Task<BankIdPaymentResult?> GetPaymentResultAsync(string configKey);
}
