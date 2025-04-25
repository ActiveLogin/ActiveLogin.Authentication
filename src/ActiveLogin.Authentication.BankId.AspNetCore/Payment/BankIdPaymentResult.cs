using System.Diagnostics.CodeAnalysis;

using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Payment;

public class BankIdPaymentResult
{
    internal BankIdPaymentResult(bool succeeded, BankIdPaymentProperties properties)
    {
        Succeeded = succeeded;
        Properties = properties;
    }

    [MemberNotNullWhen(true, [nameof(BankIdCompletionData)])]
    public bool Succeeded { get; }

    public BankIdPaymentProperties Properties { get; }

    public CompletionData? BankIdCompletionData { get; init; }

    public static BankIdPaymentResult Success(BankIdPaymentProperties properties, CompletionData bankidCompletionData)
    {
        return new BankIdPaymentResult(true, properties)
        {
            BankIdCompletionData = bankidCompletionData
        };
    }
}
