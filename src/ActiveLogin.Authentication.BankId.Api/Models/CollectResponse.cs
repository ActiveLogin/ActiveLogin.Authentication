using System;
using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    [DataContract]
    public class CollectResponse
    {

        public CollectResponse(){}

        public CollectResponse(CollectStatus status, CollectHintCode hintCode)
        {
            this.status = status.ToString();
            this.hintCode = hintCode.ToString();
        }

        [DataMember(Name = "hintCode")]
        private string hintCode { get; set; }

        public CollectHintCode HintCode
        {
            get
            {
                Enum.TryParse<CollectHintCode>(hintCode, true, out var parsedHintCode);
                return parsedHintCode;
            }
        }

        [DataMember(Name = "orderRef")]
        public string OrderRef { get; set; }

        [DataMember(Name = "status")]
        private string status { get; set; }

        public CollectStatus Status
        {
            get
            {
                Enum.TryParse<CollectStatus>(status, true, out var parsedStatus);
                return parsedStatus;
            }
        }

        [DataMember(Name = "completionData")]
        public CompletionData CompletionData { get; set; }
    }
}