using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Information about extra verifications that were part of the transaction. 
/// </summary>
public class StepUp
{
    public StepUp(bool mrtd)
    {
        Mrtd = mrtd;
    }

    /// <summary>
    /// Result of the MRTD (Machine readable travel document) verification.
    /// Mrtd true if the mrtd check was performed and passed. 
    /// Mrtd false if the mrtd check was performed and failed.  
    /// </summary>
    [JsonPropertyName("mrtd")]
    public bool Mrtd { get; }
}
