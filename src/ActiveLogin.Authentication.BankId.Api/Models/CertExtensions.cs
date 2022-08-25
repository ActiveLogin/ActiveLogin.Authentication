namespace ActiveLogin.Authentication.BankId.Api.Models;

public static class CertExtensions
{
    /// <summary>
    /// Start of validity of the users BankID.
    /// </summary>
    public static DateTimeOffset GetNotBeforeDateTime(this Cert cert)
    {
        return BankIdApiConverters.ParseUnixTimestamp(cert.NotBefore);
    }

    /// <summary>
    /// End of validity of the Users BankID.
    /// </summary>
    public static DateTimeOffset GetNotAfterDateTime(this Cert cert)
    {
        return BankIdApiConverters.ParseUnixTimestamp(cert.NotAfter);
    }
}
