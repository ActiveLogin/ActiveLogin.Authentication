using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using ActiveLogin.Authentication.BankId.Core.Certificate;

namespace ActiveLogin.Authentication.BankId.Core;

/// <summary>
/// These certificates are available in the BankID documentation and we have
/// made them available as part of Active Login with the permission from BankID.
/// </summary>
internal static class BankIdCertificates
{
    private static readonly CertificateResource BankIdApiRootCertificateProd = new() { Filename = "BankIdApiRootCertificate-Prod.crt" };
    private static readonly CertificateResource BankIdApiRootCertificateTest = new() { Filename = "BankIdApiRootCertificate-Test.crt" };
    private static readonly CertificateResource BankIdApiClientCertificateTestP12 = new() { Filename = "FPTestcert5_20240610.p12", Password = "qwerty123" };
    private static readonly CertificateResource BankIdApiClientCertificateTestPem = new() { Filename = "FPTestcert5_20240610.pem", Password = "qwerty123" };
    private static readonly CertificateResource BankIdApiClientCertificateTestPfx = new() { Filename = "FPTestcert5_20240610-legacy.pfx", Password = "qwerty123" };

    public static X509Certificate2 GetBankIdApiRootCertificateProd() => GetCertFromResourceStream(BankIdApiRootCertificateProd);
    public static X509Certificate2 GetBankIdApiRootCertificateTest() => GetCertFromResourceStream(BankIdApiRootCertificateTest);
    public static X509Certificate2 GetBankIdApiClientCertificateTest(TestCertificateFormat certificateFormat) => certificateFormat switch
    {
        TestCertificateFormat.P12 => GetCertFromResourceStream(BankIdApiClientCertificateTestP12),
        TestCertificateFormat.PEM => GetPemCertFromResourceStream(BankIdApiClientCertificateTestPem),
        TestCertificateFormat.PFX => GetCertFromResourceStream(BankIdApiClientCertificateTestPfx),
        _ => GetCertFromResourceStream(BankIdApiClientCertificateTestPfx)
    };

    private static X509Certificate2 GetCertFromResourceStream(CertificateResource resource)
    {
        return GetCertFromResourceStream(resource.Filename, resource.Password);
    }

    private static X509Certificate2 GetCertFromResourceStream(string filename, string? password = null)
    {
        var certStream = GetBankIdResourceStream(filename);
        using var memory = new MemoryStream((int)certStream.Length);
        certStream.CopyTo(memory);

        var certBytes = memory.ToArray();

        if (password is null) return X509CertificateLoader.LoadCertificate(certBytes);

        var flags = GetPlatformKeyStorageFlags();

        var cert = X509CertificateLoader.LoadPkcs12(certBytes, password, flags);

        if (!cert.HasPrivateKey)
        {
            throw new InvalidOperationException($"Certificate {filename} does not contain a private key.");
        }

        return cert;
    }

    private static X509Certificate2 GetPemCertFromResourceStream(CertificateResource resource)
    {
        var certStream = GetBankIdResourceStream(resource.Filename);
        using var streamReader = new StreamReader(certStream);
        var certAndKeyString = streamReader.ReadToEnd();

        return X509Certificate2.CreateFromEncryptedPem(certAndKeyString, certAndKeyString, resource.Password);
    }

    private static Stream GetBankIdResourceStream(string filename)
    {
        var resourceName = $"ActiveLogin.Authentication.BankId.Core.BankIdResources.{filename}";
        var assembly = typeof(BankIdCertificates).Assembly;
        return assembly.GetManifestResourceStream(resourceName)
               ?? throw new InvalidOperationException($"Can´t find resource {resourceName}");
    }

    private static X509KeyStorageFlags GetPlatformKeyStorageFlags()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable;
        }

        // Linux / Azure App Service / Containers
        return X509KeyStorageFlags.EphemeralKeySet;
    }

}
