using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;

public class BankIdEventTrigger : IBankIdEventTrigger
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
        ArgumentNullException.ThrowIfNull(nameof(bankIdEvent));

        bankIdEvent.SetContext(_bankIdActiveLoginContext);

        var tasks = new List<Task>();

        foreach (var listener in _listeners)
        {
            tasks.Add(listener.HandleAsync(bankIdEvent));
        }

        await Task.WhenAll(tasks);
    }
}
