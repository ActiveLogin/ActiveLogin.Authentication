using System.Threading.Tasks;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    public interface IBankIdEventTrigger
    {
        Task TriggerAsync(BankIdEvent bankIdEvent);
    }
}
