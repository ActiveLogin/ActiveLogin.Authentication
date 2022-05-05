using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;

internal class BankIdEventTrigger : IBankIdEventTrigger
{
    private readonly List<IBankIdEventListener> _listeners;
    private readonly BankIdActiveLoginContext _bankIdActiveLoginContext;
    
    public BankIdEventTrigger(IEnumerable<IBankIdEventListener> listeners, IOptions<BankIdActiveLoginContext> bankIdActiveLoginContext)
    {
        _listeners = listeners.ToList();
        _bankIdActiveLoginContext = bankIdActiveLoginContext.Value;
    }

    public async Task TriggerAsync(BankIdEvent bankIdEvent)
    {
        ArgumentNullException.ThrowIfNull(bankIdEvent);

        bankIdEvent.SetContext(_bankIdActiveLoginContext);

        var tasks = _listeners.Select(listener => listener.HandleAsync(bankIdEvent));

        await Task.WhenAll(tasks);
    }
}
