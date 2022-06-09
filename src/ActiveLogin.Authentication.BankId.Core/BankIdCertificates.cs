using System.Security.Cryptography.X509Certificates;

namespace ActiveLogin.Authentication.BankId.Core;

/// <summary>
/// These certificates are availible in the BankID documentation and we have
/// made them available as part of Active Login with the permission from BankID.
/// </summary>
internal static class BankIdCertificates
{
    public static readonly X509Certificate2 BankIdApiRootCertificateProd = GetCertFromResourceStream("BankIdApiRootCertificate-Prod.crt");
    public static readonly X509Certificate2 BankIdApiRootCertificateTest = GetCertFromResourceStream("BankIdApiRootCertificate-Test.crt");
    public static readonly X509Certificate2 BankIdApiClientCertificateTest = GetCertFromResourceStream("FPTestcert3_20200618.p12", "qwerty123");

    private static X509Certificate2 GetCertFromResourceStream(string filename, string? password = null)
    {
        var certStream = GetBankIdResourceStream(filename);
        using var memory = new MemoryStream((int)certStream.Length);
        certStream.CopyTo(memory);
        if (password == null)
        {
            return new X509Certificate2(memory.ToArray());
        }

        return new X509Certificate2(memory.ToArray(), password);
    }

    private static Stream GetBankIdResourceStream(string filename)
    {
        var resourceName = $"ActiveLogin.Authentication.BankId.Core.BankIdResources.{filename}";
        var assembly = typeof(BankIdCertificates).Assembly;
        return assembly.GetManifestResourceStream(resourceName)
               ?? throw new InvalidOperationException($"Can´t find resource {resourceName}");
    }
}
