using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace ActiveLogin.Authentication.BankId.AzureMonitor;

/// <summary>
/// Owns a <see cref="TelemetryConfiguration"/> and <see cref="TelemetryClient"/> created from a connection string,
/// so that buffered telemetry is flushed and the configuration is disposed when the container is disposed.
/// </summary>
internal sealed class BankIdApplicationInsightsTelemetryClientProvider : IDisposable
{
    private readonly TelemetryConfiguration _telemetryConfiguration;
    private bool _disposed;

    public BankIdApplicationInsightsTelemetryClientProvider(string connectionString)
    {
        _telemetryConfiguration = new TelemetryConfiguration
        {
            ConnectionString = connectionString
        };
        TelemetryClient = new TelemetryClient(_telemetryConfiguration);
    }

    public TelemetryClient TelemetryClient { get; }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        try
        {
            TelemetryClient.Flush();
        }
        finally
        {
            _telemetryConfiguration.Dispose();
        }
    }
}
