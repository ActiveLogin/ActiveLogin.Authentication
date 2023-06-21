namespace ActiveLogin.Authentication.BankId.Api.Models;

public static class VerifyVerificationExtensions
{
    /// <summary>
    /// Timestamp indicating date and time in UTC when the verification of the digital ID card was performed.
    /// </summary>
    /// <param name="verification"></param>
    /// <returns></returns>
    public static DateTime GetVerifiedAtDateTime(this VerifyVerification verification)
    {
        return BankIdApiConverters.ParseIso8601DateTime(verification.VerifiedAt);
    }

    /// <summary>
    /// Enveloping XAdES signature conforming to ETSI TS 103 171 v2.1.1 Baseline Profile B-B.
    /// See section Signature for detailed information about the contents of the signature.
    /// </summary>
    /// <param name="verification"></param>
    /// <returns></returns>
    public static string GetSignatureXml(this VerifyVerification verification)
    {
        return BankIdApiConverters.GetXml(verification.Signature);
    }
}
