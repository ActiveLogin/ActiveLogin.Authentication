namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

public class IncompatibleSerializationVersion : Exception
{
    public IncompatibleSerializationVersion(string type)
        : base(BankIdConstants.ErrorMessages.CouldNotDeserialize(type))
    {

    }
}
