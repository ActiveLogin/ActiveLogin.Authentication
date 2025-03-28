using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;
using ActiveLogin.Authentication.BankId.Core.Payment;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.Core.Models;

public class BankIdPaymentData
{
    public BankIdPaymentData(TransactionType transactionType, string recipientName)
    {
        TransactionType = transactionType;
        RecipientName = recipientName;
    }

    public TransactionType TransactionType { get; set; }
    public string RecipientName { get; set; }
    public Money? Money { get; set; }
    public string? RiskWarning { get; set; }
    public IEnumerable<RiskFlags>? RiskFlags { get; set; }

    public string? UserVisibleData { get; set; }
    public string? UserVisibleDataFormat { get; set; }

    public byte[]? UserNonVisibleData { get; set; }

    public PersonalIdentityNumber? RequiredPersonalIdentityNumber { get; set; }
    public bool? RequireMrtd { get; set; }
    public bool? RequirePinCode { get; set; }

    public bool? ReturnRisk { get; set; }

    public List<BankIdCertificatePolicy> CertificatePolicies { get; set; } = new();
    public CardReader? CardReader { get; set; }

    public IDictionary<string, string?> Items { get; set; } = new Dictionary<string, string?>();
}
