using ActiveLogin.Authentication.GrandId.AspNetCore.Models;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.DataProtection
{
    public interface IGrandIdLoginResultProtector
    {
        string Protect(GrandIdLoginResult loginResult);
        GrandIdLoginResult Unprotect(string protectedLoginResult);
    }
}