using ActiveLogin.Authentication.BankId.Api;

namespace ActiveLogin.Authentication.BankId.Core;

/// <summary>
/// Configuration for the simulated BankID environment.
/// </summary>
public sealed class BankIdSimulatedEnvironmentOptions
{
    /// <summary>
    /// The sequence of states returned by collect calls.
    /// </summary>
    public List<BankIdSimulatedAppApiClient.CollectState> CollectStates { get; set; } = BankIdSimulatedAppApiClient.NormalCollectStates;
}