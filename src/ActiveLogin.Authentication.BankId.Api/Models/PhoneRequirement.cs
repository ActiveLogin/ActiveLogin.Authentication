using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Requirements on how the auth or sign order must be performed.
/// </summary>
public class PhoneRequirement
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="certificatePolicies">The oid in certificate policies in the user certificate. List of String.</param>
    /// <param name="pinCode">
    /// Users are required to sign the transaction with their PIN 
    /// code, even if they have biometrics activated.
    /// 
    /// If set to true, the user is required to use pin code
    /// If set to false, the users is not required to use pin code and are allowed to use fingerprint.
    /// </param>
    public PhoneRequirement(List<string>? certificatePolicies = null, bool? pinCode = null)
    {
        CertificatePolicies = certificatePolicies;
        PinCode = pinCode;
    }

    /// <summary>
    /// The oid in certificate policies in the user certificate. List of String.
    /// </summary>
    [JsonPropertyName("certificatePolicies"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<string>? CertificatePolicies { get; }

    /// <summary>
    /// Users are required to sign the transaction with their PIN 
    /// code, even if they have biometrics activated.
    /// 
    /// If set to true, the user is required to use pin code
    /// If set to false, the users is not required to use pin code and are allowed to use fingerprint.
    /// </summary>
    [JsonPropertyName("pinCode"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? PinCode { get; }
}
