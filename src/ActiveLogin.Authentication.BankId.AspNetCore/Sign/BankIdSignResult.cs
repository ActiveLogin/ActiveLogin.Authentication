using System.Diagnostics.CodeAnalysis;

using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

public class BankIdSignResult
{
    internal BankIdSignResult(bool succeeded, BankIdSignProperties properties)
    {
        Succeeded = succeeded;
        Properties = properties;
    }

    [MemberNotNullWhen(true, new [] { nameof(BankIdCompletionData) })]
    public bool Succeeded { get; }

    public BankIdSignProperties Properties { get; }

    public CompletionData? BankIdCompletionData { get; init; }

    public BankIdSignResult Success(BankIdSignProperties properties, CompletionData bankidCompletionData)
    {
        return new BankIdSignResult(true, properties)
        {
            BankIdCompletionData = bankidCompletionData
        };
    }
}
