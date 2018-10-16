using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models
{
    [DataContract]
    public class BankIdLoginOptions
    {
        public string CertificatePolicies { get; set; } = string.Empty;
    }
}