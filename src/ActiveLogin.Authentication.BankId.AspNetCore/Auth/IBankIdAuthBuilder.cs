using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Auth;

public interface IBankIdAuthBuilder
{
    IServiceCollection Services { get; }
    AuthenticationBuilder AuthenticationBuilder { get; }
}
