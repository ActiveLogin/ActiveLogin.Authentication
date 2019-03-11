using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Cryptography
{
    internal class X509CertificateChainValidator
    {
        private readonly X509Certificate2 _certificateAuthority;

        public X509CertificateChainValidator(X509Certificate2 certificateAuthority)
        {
            _certificateAuthority = certificateAuthority;
        }

        public bool Validate(HttpRequestMessage httpRequestMessage, X509Certificate2 certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            return IsValidChain(_certificateAuthority, certificate);
        }

        private static bool IsValidChain(X509Certificate2 certificateAuthority, X509Certificate2 certificate)
        {
            X509Chain chain = GetChain(certificateAuthority);
            bool isChainValid = chain.Build(certificate);
            X509Certificate2 chainRoot = chain.ChainElements[chain.ChainElements.Count - 1].Certificate;
            bool isChainRootCertificateAuthority = chainRoot.RawData.SequenceEqual(certificateAuthority.RawData);

            return isChainValid && isChainRootCertificateAuthority;
        }

        private static X509Chain GetChain(X509Certificate2 certificateAuthority)
        {
            var chain = new X509Chain
            {
                ChainPolicy =
                {
                    RevocationMode = X509RevocationMode.NoCheck,
                    VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority
                }
            };
            chain.ChainPolicy.ExtraStore.Add(certificateAuthority);

            return chain;
        }
    }
}
