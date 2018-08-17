using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api.Models;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Persistence
{
    /// <summary>
    /// To comply with BankID Technical requirement RFT5 you need to store the data in <see cref="CompletionData"/>.
    /// </summary>
    public interface IGrandIdResultStore
    {
        //Task StoreCollectCompletedCompletionData(string orderRef, CompletionData completionData);
    }
}
