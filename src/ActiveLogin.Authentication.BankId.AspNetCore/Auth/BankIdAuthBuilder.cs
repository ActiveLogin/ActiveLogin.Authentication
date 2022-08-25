using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Auth;

internal class BankIdAuthBuilder : IBankIdAuthBuilder
{
    public IServiceCollection Services { get; }
    public AuthenticationBuilder AuthenticationBuilder { get; }

    public BankIdAuthBuilder(IServiceCollection services, AuthenticationBuilder authenticationBuilder)
    {
        Services = services;
        AuthenticationBuilder = authenticationBuilder;
    }
}
