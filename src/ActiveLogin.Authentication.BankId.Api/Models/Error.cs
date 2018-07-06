using System;
using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    [DataContract]
    public class Error
    {
        [DataMember(Name = "errorCode")]
        private string errorCode { get; set; }

        public ErrorCode ErrorCode
        {
            get
            {
                Enum.TryParse<ErrorCode>(errorCode, true, out var parsedErrorCode);
                return parsedErrorCode;
            }
        }

        [DataMember(Name = "details")]
        public string Details { get; set; }
    }
}
