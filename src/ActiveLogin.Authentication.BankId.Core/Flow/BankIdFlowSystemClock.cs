using System;

namespace ActiveLogin.Authentication.BankId.Core.Flow;

public class BankIdFlowSystemClock : IBankIdFlowSystemClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
