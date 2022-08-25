using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AzureMonitor;
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
        builder.Services.AddTransient<IBankIdEventListener>(x => new BankIdApplicationInsightsEventListener(x.GetRequiredService<TelemetryClient>(), options));

        return builder;
    }

    /// <summary>
    /// Adds an event listener that will listen for all events and write them to Application Insights
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="connectionString">Application Insights connection string</param>
    /// <param name="configureOptions">Configure logging options</param>
    /// <returns></returns>
    public static IBankIdBuilder AddApplicationInsightsEventListener(this IBankIdBuilder builder, string connectionString, Action<ApplicationInsightsBankIdEventListenerOptions> configureOptions)
    {
        var options = new ApplicationInsightsBankIdEventListenerOptions();
        configureOptions(options);
        return AddApplicationInsightsEventListener(builder, connectionString, options);
    }

    /// <summary>
    /// Adds an event listener that will listen for all events and write them to Application Insights
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="connectionString">Application Insights connection string</param>
    /// <returns></returns>
    public static IBankIdBuilder AddApplicationInsightsEventListener(this IBankIdBuilder builder, string connectionString)
    {
        var options = new ApplicationInsightsBankIdEventListenerOptions();
        return AddApplicationInsightsEventListener(builder, connectionString, options);
    }

    /// <summary>
    /// Adds an event listener that will listen for all events and write them to Application Insights
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="connectionString">Application Insights connection string</param>
    /// <param name="options">Logging options</param>
    /// <returns></returns>
    public static IBankIdBuilder AddApplicationInsightsEventListener(this IBankIdBuilder builder, string connectionString, ApplicationInsightsBankIdEventListenerOptions options)
    {
        builder.Services.AddTransient<IBankIdEventListener>(x =>
        {
            var telemetryConfiguration = new TelemetryConfiguration();
            telemetryConfiguration.ConnectionString = connectionString;
            return new BankIdApplicationInsightsEventListener(new TelemetryClient(telemetryConfiguration), options);
        });

        return builder;
    }
}
