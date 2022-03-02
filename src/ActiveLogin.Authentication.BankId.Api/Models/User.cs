using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    /// <summary>
    /// Information related to the user.
    /// </summary>
    public class User
    {
        public User(string personalIdentityNumber, string name, string givenName, string surname)
        {
            PersonalIdentityNumber = personalIdentityNumber;
            Name = name;
            GivenName = givenName;
            Surname = surname;
        }

        /// <summary>
        /// The personal number.
        /// </summary>
        [JsonPropertyName("personalNumber")]
        public string PersonalIdentityNumber { get; private set; }

        /// <summary>
        /// The given name and surname of the user.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; private set; }

        /// <summary>
        /// The given name of the user.
        /// </summary>
        [JsonPropertyName("givenName")]
        public string GivenName { get; private set; }

        /// <summary>
        /// The surname of the user.
        /// </summary>
        [JsonPropertyName("surname")]
        public string Surname { get; private set; }
    }
}
