using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    internal class BankIdAuthRequestEmptyUserDataResolver : IBankIdAuthRequestUserDataResolver
    {
        private static readonly BankIdAuthUserData _emptyAuthUserData = new();

        public BankIdAuthUserData GetUserData(BankIdAuthRequestContext authRequestContext, HttpContext httpContext)
        {
            return _emptyAuthUserData;
        }
    }
}
