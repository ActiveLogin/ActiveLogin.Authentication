using System.Threading.Tasks;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure
{
    /// <summary>
    /// Trigger an event.
    /// </summary>
    public interface IBankIdEventTrigger
    {
        Task TriggerAsync(BankIdEvent bankIdEvent);
    }
}
