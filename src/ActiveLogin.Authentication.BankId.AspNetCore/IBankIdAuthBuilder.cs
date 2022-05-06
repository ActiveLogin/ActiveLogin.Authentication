using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore;

public interface IBankIdAuthBuilder
{
    IServiceCollection Services { get; }
    AuthenticationBuilder AuthenticationBuilder { get; }
}
