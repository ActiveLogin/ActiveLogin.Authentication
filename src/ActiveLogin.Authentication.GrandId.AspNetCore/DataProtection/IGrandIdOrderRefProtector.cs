using ActiveLogin.Authentication.GrandId.AspNetCore.Models;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.DataProtection
{
    public interface IGrandIdOrderRefProtector
    {
        string Protect(GrandIdSessionId orderRef);
        GrandIdSessionId Unprotect(string protectedOrderRef);
    }
}