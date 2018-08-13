namespace ActiveLogin.Authentication.GrandId.AspNetCore.Models
{
    public class GrandIdLoginApiStatusResponse
    {
        public bool IsFinished { get; set; }

        public string StatusMessage { get; set; }
        public string RedirectUri { get; set; }

        public static GrandIdLoginApiStatusResponse Finished(string redirectUri)
        {
            return new GrandIdLoginApiStatusResponse
            {
                IsFinished = true,
                RedirectUri = redirectUri
            };
        }

        public static GrandIdLoginApiStatusResponse Pending(string statusMessage)
        {
            return new GrandIdLoginApiStatusResponse
            {
                IsFinished = false,

                StatusMessage = statusMessage
            };
        }
    }
}