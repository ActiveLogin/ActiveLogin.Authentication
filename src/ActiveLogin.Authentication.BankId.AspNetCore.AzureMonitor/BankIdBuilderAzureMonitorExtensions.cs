using System;
using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.AzureMonitor;
using ActiveLogin.Authentication.BankId.AspNetCore.Events;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BankIdBuilderAzureMonitorExtensions
    {
        public static IBankIdBuilder AddApplicationInsightsEventListener(this IBankIdBuilder builder, Action<ApplicationInsightsBankIdEventListenerOptions> configureOptions)
        {
            var options = new ApplicationInsightsBankIdEventListenerOptions();
            configureOptions(options);
            return AddApplicationInsightsEventListener(builder, options);
        }

        public static IBankIdBuilder AddApplicationInsightsEventListener(this IBankIdBuilder builder)
        {
            var options = new ApplicationInsightsBankIdEventListenerOptions();
            return AddApplicationInsightsEventListener(builder, options);
        }

        public static IBankIdBuilder AddApplicationInsightsEventListener(this IBankIdBuilder builder, ApplicationInsightsBankIdEventListenerOptions options)
        {
            builder.AuthenticationBuilder.Services.AddTransient<IBankIdEventListener>(x => new BankIdApplicationInsightsEventListener(x.GetRequiredService<TelemetryClient>(), options));

            return builder;
        }

        public static IBankIdBuilder AddApplicationInsightsEventListener(this IBankIdBuilder builder, string instrumentationKey, Action<ApplicationInsightsBankIdEventListenerOptions> configureOptions)
        {
            var options = new ApplicationInsightsBankIdEventListenerOptions();
            configureOptions(options);
            return AddApplicationInsightsEventListener(builder, instrumentationKey, options);
        }

        public static IBankIdBuilder AddApplicationInsightsEventListener(this IBankIdBuilder builder, string instrumentationKey)
        {
            var options = new ApplicationInsightsBankIdEventListenerOptions();
            return AddApplicationInsightsEventListener(builder, instrumentationKey, options);
        }

        public static IBankIdBuilder AddApplicationInsightsEventListener(this IBankIdBuilder builder, string instrumentationKey, ApplicationInsightsBankIdEventListenerOptions options)
        {
            builder.AuthenticationBuilder.Services.AddTransient<IBankIdEventListener>(x => new BankIdApplicationInsightsEventListener(new TelemetryClient(new TelemetryConfiguration(instrumentationKey)), options));

            return builder;
        }
    }
}
