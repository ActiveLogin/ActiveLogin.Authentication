using System;

namespace ActiveLogin.Authentication.BankId.Core.Flow;

public interface IBankIdFlowSystemClock
{
    public DateTimeOffset UtcNow { get; }
}
