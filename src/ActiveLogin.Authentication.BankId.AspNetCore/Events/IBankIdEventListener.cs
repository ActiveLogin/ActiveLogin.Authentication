using System.Threading.Tasks;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    public interface IBankIdEventListener
    {
        Task HandleAsync(BankIdEvent bankIdEvent);
    }
}
