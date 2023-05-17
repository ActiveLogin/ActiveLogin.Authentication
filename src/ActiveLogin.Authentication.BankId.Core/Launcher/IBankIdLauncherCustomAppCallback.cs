namespace ActiveLogin.Authentication.BankId.Core.Launcher;

public interface IBankIdLauncherCustomAppCallback
{
    public Task<bool> IsCustomApp();
    public Task<string> GetCustomAppReturnUrlScheme();
}
