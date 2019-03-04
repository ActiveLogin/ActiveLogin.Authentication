using System;
using System.Text;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public static class BankIdGetSessionUserAttributesExtensions
    {
        public static string GetSignatureXml(this BankIdGetSessionUserAttributes userAttributes)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(userAttributes.Signature));
        }

        public static DateTime GetNotBeforeDateTime(this BankIdGetSessionUserAttributes userAttributes)
        {
            return DateTime.Parse(userAttributes.NotBefore).ToUniversalTime();
        }

        public static DateTime GetNotAfterDateTime(this BankIdGetSessionUserAttributes userAttributes)
        {
            return DateTime.Parse(userAttributes.NotAfter).ToUniversalTime();
        }
    }
}