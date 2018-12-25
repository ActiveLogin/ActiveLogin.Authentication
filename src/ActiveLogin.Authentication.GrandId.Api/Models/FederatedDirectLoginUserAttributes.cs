using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public class FederatedDirectLoginUserAttributes
    {
        public FederatedDirectLoginUserAttributes(string mobilePhone, string givenName, string surname, string sameAccountName, string title)
        {
            MobilePhone = mobilePhone;
            GivenName = givenName;
            Surname = surname;
            SameAccountName = sameAccountName;
            Title = title;
        }

        [DataMember(Name = "mobile")]
        public string MobilePhone { get; private set; }

        [DataMember(Name = "givenname")]
        public string GivenName { get; private set; }

        [DataMember(Name = "sn")]
        public string Surname { get; private set; }

        [DataMember(Name = "samaccountname")]
        public string SameAccountName { get; private set; }

        [DataMember(Name = "title")]
        public string Title { get; private set; }
    }
}