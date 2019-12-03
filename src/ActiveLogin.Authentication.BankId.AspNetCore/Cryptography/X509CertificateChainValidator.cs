using System.Linq;
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

        public bool Validate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            return IsValidChain(_certificateAuthority, certificate);
        }

        private static bool IsValidChain(X509Certificate2 certificateAuthority, X509Certificate certificate)
        {
            using (var chain = GetChain(certificateAuthority))
            {
                var isChainValid = chain.Build(new X509Certificate2(certificate));
                var chainRoot = chain.ChainElements[chain.ChainElements.Count - 1].Certificate;
                var isChainRootCertificateAuthority = chainRoot.RawData.SequenceEqual(certificateAuthority.RawData);

                return isChainValid && isChainRootCertificateAuthority;
            }
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
