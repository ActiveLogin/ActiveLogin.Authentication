using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.AzureMonitor;
using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace Microsoft.Extensions.DependencyInjection;

public static class BankIdBuilderAzureMonitorExtensions
{
    /// <summary>
    /// Adds an event listener that will listen for all events and write them to Application Insights
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions">Configure logging options</param>
    /// <returns></returns>
    public static IBankIdBuilder AddApplicationInsightsEventListener(this IBankIdBuilder builder, Action<ApplicationInsightsBankIdEventListenerOptions> configureOptions)
    {
        var options = new ApplicationInsightsBankIdEventListenerOptions();
        configureOptions(options);
        return AddApplicationInsightsEventListener(builder, options);
    }

    /// <summary>
    /// Adds an event listener that will listen for all events and write them to Application Insights
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdBuilder AddApplicationInsightsEventListener(this IBankIdBuilder builder)
    {
        var options = new ApplicationInsightsBankIdEventListenerOptions();
        return AddApplicationInsightsEventListener(builder, options);
    }

    /// <summary>
    /// Adds an event listener that will listen for all events and write them to Application Insights
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options">Logging options</param>
    /// <returns></returns>
    public static IBankIdBuilder AddApplicationInsightsEventListener(this IBankIdBuilder builder, ApplicationInsightsBankIdEventListenerOptions options)
    {
        builder.AuthenticationBuilder.Services.AddTransient<IBankIdEventListener>(x => new BankIdApplicationInsightsEventListener(x.GetRequiredService<TelemetryClient>(), options));

        return builder;
    }

    /// <summary>
    /// Adds an event listener that will listen for all events and write them to Application Insights
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="instrumentationKey">Application Insights instrumentation key</param>
    /// <param name="configureOptions">Configure logging options</param>
    /// <returns></returns>
    public static IBankIdBuilder AddApplicationInsightsEventListener(this IBankIdBuilder builder, string instrumentationKey, Action<ApplicationInsightsBankIdEventListenerOptions> configureOptions)
    {
        var options = new ApplicationInsightsBankIdEventListenerOptions();
        configureOptions(options);
        return AddApplicationInsightsEventListener(builder, instrumentationKey, options);
    }

    /// <summary>
    /// Adds an event listener that will listen for all events and write them to Application Insights
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="instrumentationKey">Application Insights instrumentation key</param>
    /// <returns></returns>
    public static IBankIdBuilder AddApplicationInsightsEventListener(this IBankIdBuilder builder, string instrumentationKey)
    {
        var options = new ApplicationInsightsBankIdEventListenerOptions();
        return AddApplicationInsightsEventListener(builder, instrumentationKey, options);
    }

    /// <summary>
    /// Adds an event listener that will listen for all events and write them to Application Insights
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="instrumentationKey">Application Insights instrumentation key</param>
    /// <param name="options">Logging options</param>
    /// <returns></returns>
    public static IBankIdBuilder AddApplicationInsightsEventListener(this IBankIdBuilder builder, string instrumentationKey, ApplicationInsightsBankIdEventListenerOptions options)
    {
        builder.AuthenticationBuilder.Services.AddTransient<IBankIdEventListener>(x => new BankIdApplicationInsightsEventListener(new TelemetryClient(new TelemetryConfiguration(instrumentationKey)), options));

        return builder;
    }
}
