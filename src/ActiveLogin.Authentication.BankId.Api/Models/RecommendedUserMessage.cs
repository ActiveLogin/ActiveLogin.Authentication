using System;
using System.Collections.Generic;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    //TODO: Refactor and put behind interface + resource file?
    public class RecommendedUserMessage
    {
        private static readonly List<RecommendedUserMessage> Messages = new List<RecommendedUserMessage>()
        {
            new RecommendedUserMessage(MessageShortName.RFA1, "Starta BankID-appen", "Start your BankID app.", (data) => data.response.Status == CollectStatus.Pending && (data.response.HintCode == CollectHintCode.NoClient || (data.response.HintCode == CollectHintCode.OutstandingTransaction && !data.autoStartApp))),
            new RecommendedUserMessage(MessageShortName.RFA2, "Du har inte BankID-appen installerad. Kontakta din internetbank.", "The BankID app is not installed. Please contact your internet bank.", r => false),
            new RecommendedUserMessage(MessageShortName.RFA3, "Åtgärden avbruten. Försök igen.", "Action cancelled. Please try again.", (data) => data.response.Status == CollectStatus.Failed && data.response.HintCode == CollectHintCode.Cancelled),
            new RecommendedUserMessage(MessageShortName.RFA5, "Internt tekniskt fel. Försök igen.", "Internal error. Please try again.", r => false),
            new RecommendedUserMessage(MessageShortName.RFA6, "Åtgärden avbruten.", "Action cancelled.", (data) => data.response.Status == CollectStatus.Failed && data.response.HintCode == CollectHintCode.UserCancel),
            new RecommendedUserMessage(MessageShortName.RFA8, "BankID-appen svarar inte. Kontrollera att den är startad och att du har internetanslutning. Om du inte har något giltigt BankID kan du hämta ett hos din Bank. Försök sedan igen.", "The BankID app is not responding. Please check that the program is started and that you have internet access. If you don’t have a valid BankID you can get one from your bank. Try again.", (data) => data.response.Status == CollectStatus.Failed && data.response.HintCode == CollectHintCode.ExpiredTransaction),
            new RecommendedUserMessage(MessageShortName.RFA9, "Skriv in din säkerhetskod i BankIDappen och välj Legitimera eller Skriv under.", "Enter your security code in the BankID app and select Identify or Sign.", (data) => data.response.Status == CollectStatus.Pending && data.response.HintCode == CollectHintCode.UserSign),
            new RecommendedUserMessage(MessageShortName.RFA13, "Försöker starta BankID-appen.", "Trying to start your BankID app.", (data) => data.response.Status == CollectStatus.Pending && (data.response.HintCode == CollectHintCode.OutstandingTransaction || data.response.HintCode == CollectHintCode.NoClient) && data.autoStartApp),
            new RecommendedUserMessage(MessageShortName.RFA14A, "Söker efter BankID, det kan ta en liten stund… Om det har gått några sekunder och inget BankID har hittats har du sannolikt inget BankID som går att använda för den aktuella inloggningen/underskriften i den här datorn. Om du har ett BankIDkort, sätt in det i kortläsaren. Om du inte har något BankID kan du hämta ett hos din internetbank. Om du har ett BankID på en annan enhet kan du starta din BankID-app där.", "Searching for BankID:s, it may take a little while… If a few seconds have passed and still no BankID has been found, you probably don’t have a BankID which can be used for this login/signature on this computer. If you have a BankID card, please insert it into your card reader. If you don’t have a BankID you can order one from your internet bank. If you have a BankID on another device you can start the BankID app on that device.", (data) => data.response.Status == CollectStatus.Pending && data.response.HintCode == CollectHintCode.Started && !data.autoStartApp && data.bankIdType == BankIdType.BankId),
            new RecommendedUserMessage(MessageShortName.RFA14B, "Söker efter BankID, det kan ta en liten stund… Om det har gått några sekunder och inget BankID har hittats har du sannolikt inget BankID som går att använda för den aktuella inloggningen/underskriften i den här enheten. Om du inte har något BankID kan du hämta ett hos din internetbank. Om du har ett BankID på en annan enhet kan du starta din BankID-app där.", "Searching for BankID:s, it may take a little while… If a few seconds have passed and still no BankID has been found, you probably don’t have a BankID which can be used for this login/signature on this device. If you don’t have a BankID you can order one from your internet bank. If you have a BankID on another device you can start the BankID app on that device.", (data) => data.response.Status == CollectStatus.Pending && data.response.HintCode == CollectHintCode.Started && !data.autoStartApp && data.bankIdType == BankIdType.MobileBankId),
            new RecommendedUserMessage(MessageShortName.RFA15A, "Söker efter BankID, det kan ta en liten stund… Om det har gått några sekunder och inget BankID har hittats har du sannolikt inget BankID som går att använda för den aktuella inloggningen/underskriften i den här datorn. Om du har ett BankIDkort, sätt in det i kortläsaren. Om du inte har något BankID kan du hämta ett hos din internetbank.", "Searching for BankID:s, it may take a little while… If a few seconds have passed and still no BankID has been found, you probably don’t have a BankID which can be used for this login/signature on this computer. If you have a BankID card, please insert it into your card reader. If you don’t have a BankID you can order one from your internet bank.", (data) => data.response.Status == CollectStatus.Pending && data.response.HintCode == CollectHintCode.Started && data.autoStartApp && data.bankIdType == BankIdType.BankId),
            new RecommendedUserMessage(MessageShortName.RFA15B, "Söker efter BankID, det kan ta en liten stund… Om det har gått några sekunder och inget BankID har hittats har du sannolikt inget BankID som går att använda för den aktuella inloggningen/underskriften i den här enheten. Om du inte har något BankID kan du hämta ett hos din internetbank.", "Searching for BankID:s, it may take a little while… If a few seconds have passed and still no BankID has been found, you probably don’t have a BankID which can be used for this login/signature on this device. If you don’t have a BankID you can order one from your internet bank.", (data) => data.response.Status == CollectStatus.Pending && data.response.HintCode == CollectHintCode.Started && data.autoStartApp && data.bankIdType == BankIdType.MobileBankId),
            new RecommendedUserMessage(MessageShortName.RFA16, "Det BankID du försöker använda är för gammalt eller spärrat. Använd ett annat BankID eller hämta ett nytt hos din internetbank.", "The BankID you are trying to use is revoked or too old. Please use another BankID or order a new one from your internet bank.", (data) => data.response.Status == CollectStatus.Failed && data.response.HintCode == CollectHintCode.CertificateErr),
            new RecommendedUserMessage(MessageShortName.RFA17, "BankID-appen verkar inte finnas i din dator eller telefon. Installera den och hämta ett BankID hos din internetbank. Installera appen från https://install.bankid.com.", "The BankID app couldn’t be found on your computer or mobile device. Please install it and order a BankID from your internet bank. Install the app from install.bankid.com.", (data) => data.response.Status == CollectStatus.Failed && data.response.HintCode == CollectHintCode.StartFailed),
            new RecommendedUserMessage(MessageShortName.RFA18, "Starta BankID-appen", "Start the BankID app", r => false),
            new RecommendedUserMessage(MessageShortName.RFA19, "Vill du logga in eller skriva under med BankID på den här datorn eller med ett Mobilt BankID?", "Would you like to login or sign with a BankID on this computer or with a Mobile BankID?", r => false),
            new RecommendedUserMessage(MessageShortName.RFA20, "Vill du logga in eller skriva under med ett BankID på den här enheten eller med ett BankID på en annan enhet?", "Would you like to login or sign with a BankID on this device or with a BankID on another device?", r => false),
            new RecommendedUserMessage(MessageShortName.RFA21, "Inloggning eller signering pågår.", "Login or signing in progress.", r => false),
            new RecommendedUserMessage(MessageShortName.RFA22, "Okänt fel. Försök igen.", "Unknown error. Please try again.", (data) => data.response.Status == CollectStatus.Failed && data.response.HintCode == CollectHintCode.Unknown),
            new RecommendedUserMessage(MessageShortName.Unknown, "", "", (data) => data.response.HintCode == CollectHintCode.Unknown)
        };

        public MessageShortName ShortName { get; }
        public string SwedishText { get; }
        public string EnglishText { get; }
        private Func<(CollectResponse response, bool autoStartApp, BankIdType bankIdType), bool> RecommendedFor { get; }

        private RecommendedUserMessage(MessageShortName shortName, string swedishText, string englishText, Func<(CollectResponse response, bool autoStartApp, BankIdType bankIdType), bool> recommendedFor)
        {
            this.ShortName = shortName;
            this.SwedishText = swedishText;
            this.EnglishText = englishText;
            this.RecommendedFor = recommendedFor;
        }

        public static RecommendedUserMessage Create(MessageShortName messageShortName)
        {
            return Messages.Find(m => m.ShortName == messageShortName);
        }

        public static RecommendedUserMessage GetMessageForCollectResponse(CollectResponse response, bool autoStartApp, BankIdType bankIdType)
        {
            return Messages.Exists(m => m.RecommendedFor((response, autoStartApp, bankIdType)))
                ? Messages.Find(m => m.RecommendedFor((response, autoStartApp, bankIdType)))
                : GetMessageByShortName(MessageShortName.Unknown);
        }

        public static RecommendedUserMessage GetMessageByShortName(MessageShortName shortName)
        {
            return Messages.Find(m => m.ShortName == shortName);
        }
    }
}
