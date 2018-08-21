using System;
using System.Threading.Tasks;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    /// Dummy implementation that can be used for development and testing purposes.
    /// </summary>
    public class GrandIdDevelopmentApiClient : IGrandIdApiClient
    {
        //    new KeyValuePair<CollectStatus, CollectHintCode>(CollectStatus.Complete, CollectHintCode.UserSign)
        //};

        private readonly string _givenName;
        private readonly string _surname;
        private readonly string _name;



        public Task<SessionStateResponse> GetSessionAsync(SessionStateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponse> AuthAsync(AuthRequest request)
        {
            throw new NotImplementedException();
        }

        public void SetConfiguration(IGrandIdEnviromentConfiguration configuration)
        {
            
        }
    }
}