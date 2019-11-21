namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public interface IGrandIdEnvironmentConfiguration
    {
        string? ApiKey { get; set; }
        string? BankIdServiceKey { get; set; }
    }
}