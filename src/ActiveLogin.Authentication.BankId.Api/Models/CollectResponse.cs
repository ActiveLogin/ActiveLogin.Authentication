using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    /// <summary>
    ///     Collect response result.
    /// </summary>
    [DataContract]
    public class CollectResponse
    {
        internal CollectResponse(string orderRef, string status, string hintCode, CompletionData completionData)
        {
            OrderRef = orderRef;
            Status = status;
            HintCode = hintCode;
            CompletionData = completionData;
        }

        /// <summary>
        ///     The orderRef in question.
        /// </summary>
        [DataMember(Name = "orderRef")]
        public string OrderRef { get; private set; }

        /// <summary>
        ///     Collect status.
        /// </summary>
        [DataMember(Name = "status")]
        public string Status { get; private set; }

        /// <summary>
        ///     Collect hint code.
        ///     RP should use the HintCode to provide the user with details and instructions and keep on calling collect until
        ///     failed or complete.
        /// </summary>
        /// <remarks>Only present for pending and failed orders.</remarks>
        [DataMember(Name = "hintCode")]
        public string HintCode { get; private set; }

        /// <summary>
        ///     The completionData includes the signature, user information and the OCSP response.
        ///     RP should control the user information and continue their process.
        ///     RP should keep the completion data for future references/compliance/audit.
        /// </summary>
        /// <remarks>Only present for complete orders.</remarks>
        [DataMember(Name = "completionData")]
        public CompletionData CompletionData { get; private set; }
    }
}
