using System;

namespace ActiveLogin.Authentication.BankId.Core.Flow;

internal class BankIdFlowSystemClock : IBankIdFlowSystemClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
