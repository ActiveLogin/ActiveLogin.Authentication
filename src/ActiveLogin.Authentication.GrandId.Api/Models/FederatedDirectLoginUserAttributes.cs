using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public class FederatedDirectLoginUserAttributes
    {
        [DataMember(Name = "mobile")]
        public string MobilePhone { get; set; }

        [DataMember(Name = "givenname")]
        public string GivenName { get; set; }

        [DataMember(Name = "sn")]
        public string Surname { get; set; }

        [DataMember(Name = "samaccountname")]
        public string SameAccountName { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }
    }
}