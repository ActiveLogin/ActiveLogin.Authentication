using System;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    public static class CollectResponseExtensions
    {
        /// <summary>
        ///     Collect status.
        /// </summary>
        public static CollectStatus GetCollectStatus(this CollectResponse collectResponse)
        {
            return Enum.TryParse(collectResponse.Status, true, out CollectStatus parsedStatus)
                ? parsedStatus
                : CollectStatus.Unknown;
        }

        /// <summary>
        ///     Collect hint code.
        ///     RP should use the HintCode to provide the user with details and instructions and keep on calling collect until
        ///     failed or complete.
        /// </summary>
        /// <remarks>Only present for pending and failed orders.</remarks>
        public static CollectHintCode GetCollectHintCode(this CollectResponse collectResponse)
        {
            return Enum.TryParse(collectResponse.HintCode, true, out CollectHintCode parsedStatus)
                ? parsedStatus
                : CollectHintCode.Unknown;
        }
    }
}
