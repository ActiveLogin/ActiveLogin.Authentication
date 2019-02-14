using System;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    /// <summary>
    /// Sign request parameters.
    /// </summary>
    [DataContract]
    public class SignRequest : IAuthRequest
    {
        /// <summary></summary>
        /// <param name="endUserIp">
        /// The user IP address as seen by RP. IPv4 and IPv6 is allowed.
        /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
        /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
        ///
        /// In some use cases the IP address is not available, for instance for voice based services.
        /// In this case, the internal representation of those systems IP address is ok to use.
        /// </param>
        /// <param name="userVisibleData">
        /// The text to be displayed and signed. String. The text can be formatted using CR, LF and CRLF for new lines. The text must be encoded as UTF-8 and then base 64 encoded. 1--40 000 characters after base 64 encoding.
        /// </param>
        /// <param name="userNonVisibleData">
        /// Data not displayed to the user. String. The value must be base 64-encoded. 1-200 000charactersafter base 64-encoding.
        /// </param>
        public SignRequest(string endUserIp, string userVisibleData, string userNonVisibleData)
            : this(endUserIp, null, userVisibleData, userNonVisibleData)
        {
        }

        /// <summary></summary>
        /// <param name="endUserIp">
        /// The user IP address as seen by RP. IPv4 and IPv6 is allowed.
        /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
        /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
        ///
        /// In some use cases the IP address is not available, for instance for voice based services.
        /// In this case, the internal representation of those systems IP address is ok to use.
        /// </param>
        /// <param name="personalIdentityNumber">
        /// The personal number of the user. 12 digits, century must be included (YYYYMMDDSSSC).
        /// If the personal number is excluded, the client must be started with the AutoStartToken returned in the response.
        /// </param>
        /// <param name="userVisibleData">
        /// The text to be displayed and signed. String. The text can be formatted using CR, LF and CRLF for new lines. The text must be encoded as UTF-8 and then base 64 encoded. 1--40 000 characters after base 64 encoding.
        /// </param>
        /// <param name="userNonVisibleData">
        /// Data not displayed to the user. String. The value must be base 64-encoded. 1-200 000charactersafter base 64-encoding.
        /// </param>
        public SignRequest(string endUserIp, string personalIdentityNumber, string userVisibleData, string userNonVisibleData)
            : this(endUserIp, personalIdentityNumber, userVisibleData, userNonVisibleData, new Requirement())
        {
        }

        /// <summary></summary>
        /// <param name="endUserIp">
        /// The user IP address as seen by RP. IPv4 and IPv6 is allowed.
        /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
        /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
        ///
        /// In some use cases the IP address is not available, for instance for voice based services.
        /// In this case, the internal representation of those systems IP address is ok to use.
        /// </param>
        /// <param name="personalIdentityNumber">
        /// The personal number of the user. 12 digits, century must be included (YYYYMMDDSSSC).
        /// If the personal number is excluded, the client must be started with the AutoStartToken returned in the response.
        /// </param>
        /// <param name="userVisibleData">
        /// The text to be displayed and signed. String. The text can be formatted using CR, LF and CRLF for new lines. The text must be encoded as UTF-8 and then base 64 encoded. 1--40 000 characters after base 64 encoding.
        /// </param>
        /// <param name="userNonVisibleData">
        /// Data not displayed to the user. String. The value must be base 64-encoded. 1-200 000charactersafter base 64-encoding.
        /// </param>
        /// <param name="requirement">Requirements on how the auth or sign order must be performed.</param>
        public SignRequest(string endUserIp, string personalIdentityNumber, string userVisibleData, string userNonVisibleData, Requirement requirement)
        {
            EndUserIp = endUserIp;
            PersonalIdentityNumber = personalIdentityNumber;
            Requirement = requirement;
            UserVisibleData = EnsureBase64EncodedString(userVisibleData);
            UserNonVisibleData = EnsureBase64EncodedString(userNonVisibleData);
        }

        /// <summary>
        /// The user IP address as seen by RP. IPv4 and IPv6 is allowed.
        /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
        /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
        ///
        /// In some use cases the IP address is not available, for instance for voice based services.
        /// In this case, the internal representation of those systems IP address is ok to use.
        /// </summary>
        [DataMember(Name = "endUserIp")]
        public string EndUserIp { get; private set; }

        /// <summary>
        /// The personal number of the user. 12 digits, century must be included (YYYYMMDDSSSC).
        /// If the personal number is excluded, the client must be started with the AutoStartToken returned in the response.
        /// </summary>
        [DataMember(Name = "personalNumber", EmitDefaultValue = false)]
        public string PersonalIdentityNumber { get; private set; }

        /// <summary>
        /// Requirements on how the auth or sign order must be performed.
        /// </summary>
        [DataMember(Name = "requirement", EmitDefaultValue = false)]
        public Requirement Requirement { get; private set; }

        /// <summary>
        /// The text to be displayed and signed. String. The text can be formatted using CR, LF and CRLF for new lines.
        /// The text must be encoded as UTF-8 and then base 64 encoded. 1--40 000 characters after base 64 encoding.
        /// </summary>
        [DataMember(Name = "userVisibleData")]
        public string UserVisibleData { get; private set; }

        /// <summary>
        /// Data not displayed to the user. String. The value must be base 64-encoded. 1-200 000 characters after base 64-encoding.
        /// </summary>
        [DataMember(Name = "userNonVisibleData", EmitDefaultValue = false)]
        public string UserNonVisibleData { get; private set; }

        private static string EnsureBase64EncodedString(string value)
        {
            if (value == null) {
                return null;
            }
            if (IsBase64String(value)) return value;

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

        private static bool IsBase64String(string value)
        {
            if (value == null) return false;
            value = value.Trim();
            return (value.Length % 4 == 0) && Regex.IsMatch(value, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }
    }
}
