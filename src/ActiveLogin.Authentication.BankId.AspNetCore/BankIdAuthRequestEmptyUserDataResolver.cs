using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    internal class BankIdAuthRequestEmptyUserDataResolver : IBankIdAuthRequestUserDataResolver
    {
        private static readonly BankIdAuthUserData _emptyAuthUserData = new();

        public async Task<BankIdAuthUserData> GetUserDataAsync(BankIdAuthRequestContext authRequestContext, HttpContext httpContext)
        {
            return await Task.FromResult(_emptyAuthUserData);
        }
    }
}
