using System.Threading.Tasks;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure
{
    public interface IBankIdEventListener
    {
        Task HandleAsync(BankIdEvent bankIdEvent);
    }
}
