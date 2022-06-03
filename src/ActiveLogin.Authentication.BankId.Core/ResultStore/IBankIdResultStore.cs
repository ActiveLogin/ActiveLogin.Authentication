using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Core.ResultStore;

/// <summary>
/// To comply with BankID Technical requirement RFT5 you need to store the data in <see cref="CompletionData"/>.
/// </summary>
public interface IBankIdResultStore
{
    Task StoreCollectCompletedCompletionData(string orderRef, CompletionData completionData);
}
