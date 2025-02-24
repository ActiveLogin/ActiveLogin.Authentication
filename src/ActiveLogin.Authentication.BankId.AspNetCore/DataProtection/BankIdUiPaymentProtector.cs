using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdUiPaymentProtector(
    IDataProtectionProvider dataProtectionProvider
) : BankIdDataStateProtector<Payment.BankIdUiPaymentState>(dataProtectionProvider, new BankIdUiPaymentStateSerializer()),
    IBankIdDataStateProtector<Payment.BankIdUiPaymentState>
{
}
