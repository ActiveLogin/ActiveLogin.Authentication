using System;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Flow;

internal class BankIdFlowSystemClock : IBankIdFlowSystemClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
