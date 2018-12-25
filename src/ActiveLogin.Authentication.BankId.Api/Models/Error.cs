using System;
using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    [DataContract]
    public class Error
    {
        public Error(string errorCode, string details)
        {
            ErrorCodeRaw = errorCode;
            Details = details;
        }

        [DataMember(Name = "errorCode")]
        public string ErrorCodeRaw { get; private set; }
        public ErrorCode ErrorCode => Enum.TryParse<ErrorCode>(ErrorCodeRaw, true, out var errorCode) ? errorCode : ErrorCode.Unknown;

        [DataMember(Name = "details")]
        public string Details { get; private set; }
    }
}
