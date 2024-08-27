namespace ActiveLogin.Authentication.BankId.Core.Certificate;

public enum TestCertificateFormat
{
    /// <summary>
    /// This file stores the certificate and the private key in a PKCS#12 format.
    /// It is encrypted using the AES-256-CBC-algorithm, which has a higher security level than older methods.
    /// When you create your certificate for production with the BankID Keygen it will be created in this format.
    /// We recommend that you select this format if possible.
    /// </summary>
    P12,

    /// <summary>
    /// This file contains the certificate and the encrypted private key in a PEM format.
    /// The certificate is placed in the beginning of the file, followed by the private key.
    /// Use this if your application requires PEM format.
    /// </summary>
    PEM,

    /// <summary>
    /// This file stores the certificate and the private key in a PKCS#12 format.
    /// For compatibility reasons it’s encrypted using the older algorithm ”pbeWithSHA1And40BitRC2-CBC”.
    /// This encryption method is considered weak and should only be used for older applications that don’t support modern algorithms,
    /// such as Windows Server earlier versions than 2022.
    /// </summary>
    PFX
}
