using System.Threading.Tasks;

namespace ActiveLogin.Authentication.BankId.Core;

public class BankIdAuthRequestEmptyUserDataResolver : IBankIdAuthRequestUserDataResolver
{
    private static readonly BankIdAuthUserData EmptyAuthUserData = new();

    public async Task<BankIdAuthUserData> GetUserDataAsync(BankIdAuthRequestContext authRequestContext)
    {
        return await Task.FromResult(EmptyAuthUserData);
    }
}
