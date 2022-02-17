using System;
using System.Runtime.Serialization;
using System.Text;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    /// <summary>
    /// Auth request parameters.
    /// </summary>
    [DataContract]
    public class AuthRequest
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
        public AuthRequest(string endUserIp)
            : this(endUserIp, null, null, null, new Requirement(), null)
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
        public AuthRequest(string endUserIp, string personalIdentityNumber)
            : this(endUserIp, null, null, personalIdentityNumber, new Requirement(), null)
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
        /// <param name="userVisibleData">
        /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
        /// </param>
        /// <param name="personalIdentityNumber">
        /// The personal number of the user. 12 digits, century must be included (YYYYMMDDSSSC).
        /// If the personal number is excluded, the client must be started with the AutoStartToken returned in the response.
        /// </param>
        public AuthRequest(string endUserIp, string userVisibleData, string personalIdentityNumber)
            : this(endUserIp, userVisibleData, null, personalIdentityNumber, new Requirement(), null)
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
        /// <param name="requirement">Requirements on how the auth or sign order must be performed.</param>
        public AuthRequest(string endUserIp, string? personalIdentityNumber, Requirement requirement)
            : this(endUserIp, null, null, personalIdentityNumber, requirement, null)
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
        /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
        /// </param>
        /// <param name="requirement">Requirements on how the auth or sign order must be performed.</param>
        public AuthRequest(string endUserIp, string? userVisibleData, string? personalIdentityNumber, Requirement requirement)
            : this(endUserIp, userVisibleData, null, personalIdentityNumber, requirement, null)
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
        /// <param name="requirement">Requirements on how the auth or sign order must be performed.</param>
        /// <param name="userVisibleData">
        /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
        /// </param>
        /// <param name="userNonVisibleData">
        /// Data not displayed to the user. String. The value must be base 64-encoded. 1-1 500 characters after base 64-encoding
        /// </param>
        public AuthRequest(string endUserIp, string? userVisibleData, byte[]? userNonVisibleData, string? personalIdentityNumber, Requirement? requirement)
            : this(endUserIp, userVisibleData, userNonVisibleData, personalIdentityNumber, requirement, null)
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
        /// <param name="userVisibleData">
        /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
        /// </param>
        /// <param name="userNonVisibleData">
        /// Data not displayed to the user.
        /// </param>
        /// <param name="personalIdentityNumber">
        /// The personal number of the user. 12 digits, century must be included (YYYYMMDDSSSC).
        /// If the personal number is excluded, the client must be started with the AutoStartToken returned in the response.
        /// </param>
        /// <param name="requirement">Requirements on how the auth or sign order must be performed.</param>
        /// <param name="userVisibleDataFormat">
        /// If present, and set to "simpleMarkdownV1", this parameter indicates that userVisibleData holds formatting characters which, if used correctly, will make the text displayed with the user nicer to look at.
        /// For further information of formatting options, please study the document Guidelines for Formatted Text.
        /// </param>
        public AuthRequest(string endUserIp, string? userVisibleData, byte[]? userNonVisibleData, string? personalIdentityNumber, Requirement? requirement, string? userVisibleDataFormat)
        {
            EndUserIp = endUserIp ?? throw new ArgumentNullException(nameof(endUserIp));
            UserVisibleData = ToBase64EncodedString(userVisibleData);
            PersonalIdentityNumber = personalIdentityNumber;
            UserNonVisibleData = ToBase64EncodedString(userNonVisibleData);
            Requirement = requirement ?? new Requirement();
            UserVisibleDataFormat = userVisibleDataFormat;
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
        public string? PersonalIdentityNumber { get; private set; }

        /// <summary>
        /// Requirements on how the auth or sign order must be performed.
        /// </summary>
        [DataMember(Name = "requirement", EmitDefaultValue = false)]
        public Requirement Requirement { get; private set; }

        /// <summary>
        /// A text that is displayed to the user during authentication with BankID, with the
        /// purpose of providing context for the authentication and to enable users to notice if
        /// there is something wrong about the identification and avoid attempted frauds.The
        /// text can be formatted using CR, LF and CRLF for new lines.The text must be
        /// encoded as UTF-8 and then base 64 encoded. 1—1 500 characters after base 64encoding.
        /// </summary>
        [DataMember(Name = "userVisibleData", EmitDefaultValue = false)]
        public string? UserVisibleData { get; private set; }

        /// <summary>
        /// Data not displayed to the user. String. The value must be base 64-encoded. 1-1 500
        /// characters after base 64-encoding
        /// </summary>
        [DataMember(Name = "userNonVisibleData", EmitDefaultValue = false)]
        public string? UserNonVisibleData { get; private set; }

        /// <summary>
        /// If present, and set to “simpleMarkdownV1”, this parameter indicates that
        /// userVisibleData holds formatting characters which, if used correctly, will make
        /// the text displayed with the user nicer to look at.For further information of
        /// formatting options, please study the document Guidelines for Formatted Text.
        /// </summary>
        [DataMember(Name = "userVisibleDataFormat", EmitDefaultValue = false)]
        public string? UserVisibleDataFormat { get; private set; }

        private static string? ToBase64EncodedString(string? value)
        {
            if (value == null)
            {
                return null;
            }

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

        private static string? ToBase64EncodedString(byte[]? value)
        {
            if (value == null)
            {
                return null;
            }

            return Convert.ToBase64String(value);
        }
    }
}
