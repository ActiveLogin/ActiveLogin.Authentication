using System;
using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public class ErrorObject
    {
        public ErrorObject(string codeRaw, string message)
        {
            CodeRaw = codeRaw;
            Message = message;
        }

        [DataMember(Name = "code")]
        public string CodeRaw { get; private set; }
        public ErrorCode Code => Enum.TryParse<ErrorCode>(CodeRaw, true, out var errorCode) ? errorCode : ErrorCode.Unknown;

        [DataMember(Name = "message")]
        public string Message { get; private set; }
    }
}