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
        internal CollectResponse(string orderRef, string statusRaw, string hintCodeRaw, CompletionData completionData)
        {
            OrderRef = orderRef;
            StatusRaw = statusRaw;
            HintCodeRaw = hintCodeRaw;
            CompletionData = completionData;
        }

        /// <summary>
        /// The orderRef in question.
        /// </summary>
        [DataMember(Name = "orderRef")]
        public string OrderRef { get; private set; }

        [DataMember(Name = "status")]
        public string StatusRaw { get; private set; }

        /// <summary>
        /// Collect status.
        /// </summary>
        public CollectStatus Status => Enum.TryParse<CollectStatus>(StatusRaw, true, out var parsedStatus) ? parsedStatus : CollectStatus.Unknown;

        [DataMember(Name = "hintCode")]
        public string HintCodeRaw { get; private set; }

        /// <summary>
        /// Collect hint code.
        /// RP should use the HintCode to provide the user with details and instructions and keep on calling collect until failed or complete.
        /// </summary>
        /// <remarks>Only present for pending and failed orders.</remarks>
        public CollectHintCode HintCode => Enum.TryParse<CollectHintCode>(HintCodeRaw, true, out var parsedHintCode) ? parsedHintCode : CollectHintCode.Unknown;

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