using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    [DataContract]
    public class AuthRequest
    {
        public AuthRequest(string endUserIp)
            : this(endUserIp, null)
        {
        }

        public AuthRequest(string endUserIp, string personalIdentityNumber)
        {
            EndUserIp = endUserIp;
            PersonalIdentityNumber = personalIdentityNumber;
        }

        [DataMember(Name = "endUserIp")] public string EndUserIp { get; set; }

        [DataMember(Name = "personalNumber", EmitDefaultValue = false)]
        public string PersonalIdentityNumber { get; set; }

        [DataMember(Name = "requirement", EmitDefaultValue = false)]
        public Requirement Requirement { get; set; }
    }
}