﻿namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public class BankIdLoginApiStatusResponse
    {
        private BankIdLoginApiStatusResponse(bool isFinished, string statusMessage, string redirectUri)
        {
            IsFinished = isFinished;
            StatusMessage = statusMessage;
            RedirectUri = redirectUri;
        }


        public bool IsFinished { get; }
        public string StatusMessage { get; }
        public string RedirectUri { get; }


        public static BankIdLoginApiStatusResponse Finished(string redirectUri)
        {
            return new BankIdLoginApiStatusResponse(true, null, redirectUri);
        }

        public static BankIdLoginApiStatusResponse Pending(string statusMessage)
        {
            return new BankIdLoginApiStatusResponse(false, statusMessage, null);
        }
    }
}