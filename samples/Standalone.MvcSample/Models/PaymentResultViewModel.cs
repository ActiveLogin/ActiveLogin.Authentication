namespace Standalone.MvcSample.Models;

public class PaymentResultViewModel
{
    public PaymentResultViewModel(string personalIdentityNumber, string name, string ipAddress, string transactionType, string recipientName)
    {
        PersonalIdentityNumber = personalIdentityNumber;
        Name = name;
        IpAddress = ipAddress;
        TransactionType = transactionType;
        RecipientName = recipientName;
    }

    public string PersonalIdentityNumber { get; }
    public string Name { get; }
    public string IpAddress { get; }
    public string TransactionType { get; }
    public string RecipientName { get; }
}
