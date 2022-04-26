using System;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public class BankIdQrStartState
    {
        public BankIdQrStartState(DateTimeOffset qrStartTime, string qrStartToken, string qrStartSecret)
        {
            QrStartTime = qrStartTime;
            QrStartToken = qrStartToken;
            QrStartSecret = qrStartSecret;
        }

        public DateTimeOffset QrStartTime { get; }
        public string QrStartToken { get; }
        public string QrStartSecret { get; }
    }
}
