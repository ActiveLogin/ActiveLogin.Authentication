using System.Threading.Tasks;

namespace ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;

public interface IBankIdEventListener
{
    Task HandleAsync(BankIdEvent bankIdEvent);
}
