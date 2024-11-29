namespace ActiveLogin.Authentication.BankId.Core.Requirements;

public class BankIdAuthRequestEmptyRequirementsResolver : IBankIdAuthRequestRequirementsResolver
{
    public static readonly BankIdAuthRequirements EmptyAuthRequirements = new();

    public Task<BankIdAuthRequirements> GetRequirementsAsync()
    {
        return Task.FromResult(EmptyAuthRequirements);
    }
}
