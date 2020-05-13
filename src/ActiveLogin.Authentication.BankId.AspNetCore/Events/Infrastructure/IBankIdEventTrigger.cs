using System.Threading.Tasks;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure
{
    public interface IBankIdEventTrigger
    {
        Task TriggerAsync(BankIdEvent bankIdEvent);
    }
}
