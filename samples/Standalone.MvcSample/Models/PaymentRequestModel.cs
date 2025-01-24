namespace Standalone.MvcSample.Models;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class PaymentRequestModel
{
    public string ReturnUrl { get; set; }

    public string TransactionType { get; set; }

    public string RecipientName { get; set; }
}
