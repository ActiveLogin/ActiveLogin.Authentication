namespace Standalone.MvcSample.Models;

public class PaymentResultViewModel
{
    public PaymentResultViewModel(string personalIdentityNumber, string name, string ipAddress, string transactionType, string recipientName, string? amount = null, string? currency = null)
    {
        PersonalIdentityNumber = personalIdentityNumber;
        Name = name;
        IpAddress = ipAddress;
        TransactionType = transactionType;
        RecipientName = recipientName;
        Amount = amount;
        Currency = currency;
    }

    public string PersonalIdentityNumber { get; }
    public string Name { get; }
    public string IpAddress { get; }
    public string TransactionType { get; }
    public string RecipientName { get; }
    public string? Amount { get; }
    public string? Currency { get; }
}
