using System.Threading.Tasks;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    internal class BankIdAuthRequestEmptyUserDataResolver : IBankIdAuthRequestUserDataResolver
    {
        private static readonly BankIdAuthUserData _emptyAuthUserData = new();

        public async Task<BankIdAuthUserData> GetUserDataAsync(BankIdAuthRequestContext authRequestContext)
        {
            return await Task.FromResult(_emptyAuthUserData);
        }
    }
}
