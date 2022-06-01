using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace ActiveLogin.Authentication.BankId.Core.Cryptography;

internal class X509CertificateChainValidator
{
    private readonly X509Certificate2 _certificateAuthority;

    public X509CertificateChainValidator(X509Certificate2 certificateAuthority)
    {
        _certificateAuthority = certificateAuthority;
    }

    // Inspired by: https://www.meziantou.net/custom-certificate-validation-in-dotnet.htm#dotnet-5-way-of-vali
    public bool Validate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
    {
        ArgumentNullException.ThrowIfNull(certificate);

        if ((sslPolicyErrors & ~SslPolicyErrors.RemoteCertificateChainErrors) != 0)
        {
            return false;
        }

        if (chain == null)
        {
            return false;
        }

        chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
        chain.ChainPolicy.CustomTrustStore.Clear();
        chain.ChainPolicy.CustomTrustStore.Add(_certificateAuthority);

        return chain.Build((X509Certificate2)certificate);
    }
}
