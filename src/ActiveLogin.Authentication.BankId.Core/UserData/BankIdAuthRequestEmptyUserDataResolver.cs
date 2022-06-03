namespace ActiveLogin.Authentication.BankId.Core.UserData;

public class BankIdAuthRequestEmptyUserDataResolver : IBankIdAuthRequestUserDataResolver
{
    private static readonly BankIdAuthUserData EmptyAuthUserData = new();

    public async Task<BankIdAuthUserData> GetUserDataAsync(BankIdAuthRequestContext authRequestContext)
    {
        return await Task.FromResult(EmptyAuthUserData);
    }
}
