namespace ActiveLogin.Authentication.BankId.Core.UserData;

public class BankIdAuthRequestStaticUserDataResolver : IBankIdAuthRequestUserDataResolver
{
    private readonly BankIdAuthUserData _authUserData;

    public BankIdAuthRequestStaticUserDataResolver(BankIdAuthUserData authUserData)
    {
        _authUserData = authUserData;
    }

    public async Task<BankIdAuthUserData> GetUserDataAsync(BankIdAuthRequestContext authRequestContext)
    {
        return await Task.FromResult(_authUserData);
    }
}
