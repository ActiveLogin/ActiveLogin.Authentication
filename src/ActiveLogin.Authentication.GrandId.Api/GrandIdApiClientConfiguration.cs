namespace ActiveLogin.Authentication.GrandId.Api
{
    public class GrandIdApiClientConfiguration
    {
        public GrandIdApiClientConfiguration(string apiKey, string bankIdServiceKey = null)
        {
            ApiKey = apiKey;
            BankIdServiceKey = bankIdServiceKey;
        }

        public string ApiKey { get; }
        public string BankIdServiceKey { get; }
    }
}
