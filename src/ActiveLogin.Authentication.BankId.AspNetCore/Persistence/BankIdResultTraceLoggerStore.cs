using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api.Models;
using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Persistence
{
    /// <summary>
    /// Will store the completion data as a trace log
    /// </summary>
    public class BankIdResultTraceLoggerStore : IBankIdResultStore
    {
        private readonly EventId _eventId = new EventId(101, "StoreCollectCompletedCompletionData");
        private readonly ILogger<BankIdResultTraceLoggerStore> _logger;

        public BankIdResultTraceLoggerStore(ILogger<BankIdResultTraceLoggerStore> logger)
        {
            _logger = logger;
        }

        public Task StoreCollectCompletedCompletionData(string orderRef, CompletionData completionData)
        {
            _logger.LogTrace(_eventId, "Storing completion data for OrderRef '{OrderRef}' (UserPersonalIdentityNumber: '{UserPersonalIdentityNumber}'; UserGivenName: '{UserGivenName}'; UserSurname: '{UserSurname}'; UserName: '{UserName}'; Signature: '{Signature}'; OcspResponse: '{OcspResponse}')", orderRef, completionData.User.PersonalIdentityNumber, completionData.User.GivenName, completionData.User.Surname, completionData.User.Name, completionData.SignatureRaw, completionData.OcspResponse);

            return Task.CompletedTask;
        }
    }
}