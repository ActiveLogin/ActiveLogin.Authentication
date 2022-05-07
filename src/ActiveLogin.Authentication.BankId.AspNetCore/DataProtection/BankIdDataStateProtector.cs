using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal abstract class BankIdDataStateProtector<TModel>
{
    private const string ProtectorVersion = "v2";

    private readonly ISecureDataFormat<TModel> _secureDataFormat;

    protected BankIdDataStateProtector(
        IDataProtectionProvider dataProtectionProvider,
        IDataSerializer<TModel> dataSerializer
    )
    {
        var dataProtector = dataProtectionProvider.CreateProtector(
            typeof(TModel).FullName ?? typeof(TModel).Name,
            ProtectorVersion
        );

        _secureDataFormat = new SecureDataFormat<TModel>(
            dataSerializer,
            dataProtector
        );
    }

    public string Protect(TModel model)
    {
        return _secureDataFormat.Protect(model);
    }

    public TModel Unprotect(string protectedModel)
    {
        var unprotected = _secureDataFormat.Unprotect(protectedModel);

        if (unprotected == null)
        {
            throw new Exception(BankIdConstants.ErrorMessages.CouldNotUnprotect(typeof(TModel).Name));
        }

        return unprotected;
    }
}
