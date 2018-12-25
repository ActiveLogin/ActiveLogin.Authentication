using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    [DataContract]
    public class Error
    {
        internal Error(string errorCode, string details)
        {
            ErrorCode = errorCode;
            Details = details;
        }

        [DataMember(Name = "errorCode")]
        public string ErrorCode { get; private set; }

        [DataMember(Name = "details")]
        public string Details { get; private set; }
    }
}
