using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    internal class BankIdAuthRequestStaticUserDataResolver : IBankIdAuthRequestUserDataResolver
    {
        private readonly BankIdAuthUserData _authUserData;

        public BankIdAuthRequestStaticUserDataResolver(BankIdAuthUserData authUserData)
        {
            _authUserData = authUserData;
        }

        public async Task<BankIdAuthUserData> GetUserDataAsync(BankIdAuthRequestContext authRequestContext, HttpContext httpContext)
        {
            return await Task.FromResult(_authUserData);
        }
    }
}
