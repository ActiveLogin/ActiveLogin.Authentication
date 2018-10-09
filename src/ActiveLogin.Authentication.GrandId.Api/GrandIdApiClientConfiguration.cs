namespace ActiveLogin.Authentication.GrandId.Api
{
    public class GrandIdApiClientConfiguration
    {
        public GrandIdApiClientConfiguration(string apiKey)
        {
            ApiKey = apiKey;
        }

        public string ApiKey { get; }
    }
}