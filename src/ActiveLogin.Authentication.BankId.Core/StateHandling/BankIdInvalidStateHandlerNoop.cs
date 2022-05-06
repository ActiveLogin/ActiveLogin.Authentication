namespace ActiveLogin.Authentication.BankId.Core.StateHandling;

internal class BankIdInvalidStateHandlerNoop : IBankIdInvalidStateHandler
{
    public Task HandleAsync(BankIdInvalidStateContext invalidStateContext)
    {
        return Task.CompletedTask;
    }
}
