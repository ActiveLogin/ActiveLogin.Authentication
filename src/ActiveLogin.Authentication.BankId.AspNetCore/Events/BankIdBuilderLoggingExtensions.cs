using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.Events;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BankIdBuilderLoggingExtensions
    {
        public static IBankIdBuilder AddLogger(this IBankIdBuilder builder)
        {
            var services = builder.AuthenticationBuilder.Services;

            services.AddTransient<IBankIdEventListener, LoggerBankIdEventListner>();

            return builder;
        }
    }
}
