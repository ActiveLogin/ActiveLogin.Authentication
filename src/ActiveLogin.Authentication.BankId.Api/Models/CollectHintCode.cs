namespace ActiveLogin.Authentication.BankId.Api.Models
{
    public enum CollectHintCode
    {
        Unknown,

        //Pending
        OutstandingTransaction,
        NoClient,
        Started,
        UserSign,

        //Error
        ExpiredTransaction,
        CertificateErr,
        UserCancel,
        Cancelled,
        StartFailed
    }
}