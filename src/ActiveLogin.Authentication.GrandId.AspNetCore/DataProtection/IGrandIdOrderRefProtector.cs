using ActiveLogin.Authentication.GrandId.AspNetCore.Models;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.DataProtection
{
    public interface IGrandIdOrderRefProtector
    {
        string Protect(GrandIdOrderRef orderRef);
        GrandIdOrderRef Unprotect(string protectedOrderRef);
    }
}