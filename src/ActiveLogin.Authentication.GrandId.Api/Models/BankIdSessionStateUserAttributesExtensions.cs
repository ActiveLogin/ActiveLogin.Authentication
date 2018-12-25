using System;
using System.Text;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public static class BankIdSessionStateUserAttributesExtensions
    {
        public static string GetSignatureXml(this BankIdSessionStateUserAttributes userAttributes)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(userAttributes.Signature));
        }
        
        public static DateTime GetNotBeforeDateTime(this BankIdSessionStateUserAttributes userAttributes)
        {
            return DateTime.Parse(userAttributes.NotBefore).ToUniversalTime();
        }

        public static DateTime GetNotAfterDateTime(this BankIdSessionStateUserAttributes userAttributes)
        {
            return DateTime.Parse(userAttributes.NotAfter).ToUniversalTime();
        }
    }
}