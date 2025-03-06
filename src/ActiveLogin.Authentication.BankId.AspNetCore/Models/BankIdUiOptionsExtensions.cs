using ActiveLogin.Authentication.BankId.Core.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models;

public static class BankIdUiOptionsExtensions
{
    public static BankIdFlowOptions ToBankIdFlowOptions(this BankIdUiOptions options) => new(
        options.CertificatePolicies,
        options.AllowedRiskLevel,
        options.SameDevice,
        options.RequirePinCode,
        options.RequireMrtd,
        options.ReturnRisk,
        cardReader: options.CardReader
    );
}
