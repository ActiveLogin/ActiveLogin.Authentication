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

        public BankIdAuthUserData GetUserData(BankIdAuthRequestContext authRequestContext, HttpContext httpContext)
        {
            return _authUserData;
        }
    }
}
