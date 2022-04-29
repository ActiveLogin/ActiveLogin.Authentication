using System;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Flow;

public interface IBankIdFlowSystemClock
{
    public DateTimeOffset UtcNow { get; }
}
