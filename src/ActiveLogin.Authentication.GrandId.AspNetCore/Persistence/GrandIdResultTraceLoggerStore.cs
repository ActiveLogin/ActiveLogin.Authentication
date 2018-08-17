using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api.Models;
using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Persistence
{
    /// <summary>
    /// Will store the completion data as a trace log
    /// </summary>
    public class GrandIdResultTraceLoggerStore : IGrandIdResultStore
    {
        private readonly EventId _eventId = new EventId(101, "StoreCollectCompletedCompletionData");
        private readonly ILogger<GrandIdResultTraceLoggerStore> _logger;

        public GrandIdResultTraceLoggerStore(ILogger<GrandIdResultTraceLoggerStore> logger)
        {
            _logger = logger;
        }

        //public Task StoreCollectCompletedCompletionData(string orderRef, CompletionData completionData)
        //{
        //    _logger.LogTrace(_eventId, "Storing completion data for OrderRef '{OrderRef}' (UserPersonalIdentityNumber: '{UserPersonalIdentityNumber}'; UserGivenName: '{UserGivenName}'; UserSurname: '{UserSurname}'; UserName: '{UserName}'; Signature: '{Signature}'; OcspResponse: '{OcspResponse}')", orderRef, completionData.User.PersonalIdentityNumber, completionData.User.GivenName, completionData.User.Surname, completionData.User.Name, completionData.Signature, completionData.OcspResponse);

        //    return Task.CompletedTask;
        //}
    }
}