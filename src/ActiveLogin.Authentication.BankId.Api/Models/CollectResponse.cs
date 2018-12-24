using System;
using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    /// <summary>
    /// Collect response result.
    /// </summary>
    [DataContract]
    public class CollectResponse
    {
        public CollectResponse()
        {

        }

        public CollectResponse(string orderRef, string status, string hintCode, CompletionData completionData)
        {
            OrderRef = orderRef;
            this.status = status;
            this.hintCode = hintCode;
            CompletionData = completionData;
        }

        /// <summary>
        /// The orderRef in question.
        /// </summary>
        [DataMember(Name = "orderRef")]
        public string OrderRef { get; private set; }

        [DataMember(Name = "status")]
        private string status { get; set; }

        /// <summary>
        /// Collect status.
        /// </summary>
        public CollectStatus Status
        {
            get
            {
                Enum.TryParse<CollectStatus>(status, true, out var parsedStatus);
                return parsedStatus;
            }
        }

        [DataMember(Name = "hintCode")]
        private string hintCode { get; set; }

        /// <summary>
        /// Collect hint code.
        /// RP should use the HintCode to provide the user with details and instructions and keep on calling collect until failed or complete.
        /// </summary>
        /// <remarks>Only present for pending and failed orders.</remarks>
        public CollectHintCode HintCode
        {
            get
            {
                Enum.TryParse<CollectHintCode>(hintCode, true, out var parsedHintCode);
                return parsedHintCode;
            }
        }

        /// <summary>
        /// The completionData includes the signature, user information and the OCSP response.
        /// RP should control the user information and continue their process.
        /// RP should keep the completion data for future references/compliance/audit.
        /// </summary>
        /// <remarks>Only present for complete orders.</remarks>
        [DataMember(Name = "completionData")]
        public CompletionData CompletionData { get; private set; }
    }
}