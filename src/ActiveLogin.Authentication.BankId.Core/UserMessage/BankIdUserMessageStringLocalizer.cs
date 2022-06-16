using ActiveLogin.Authentication.BankId.Api.UserMessage;

namespace ActiveLogin.Authentication.BankId.Core.UserMessage;

public class BankIdUserMessageLocalizer : IBankIdUserMessageLocalizer
{
    private const string SwedishTwoLetterIsoLanguageName = "sv";

    private readonly Dictionary<MessageShortName, Translation> _translations = new()
    {
        { MessageShortName.RFA1, new("Starta BankID-appen.", "Start your BankID app.") },
        { MessageShortName.RFA1QR, new("Starta BankID-appen och scanna QR-koden.", "Start your BankID app and scan the QR code.") },
        { MessageShortName.RFA13, new("Försöker starta BankID-appen.", "Trying to start your BankID app.") },
        { MessageShortName.RFA14A, new("Söker efter BankID, det kan ta en liten stund… Om det har gått några sekunder och inget BankID har hittats har du sannolikt inget BankID som går att använda för den aktuella identifieringen/underskriften i den här datorn. Om du har ett BankID-kort, sätt in det i kortläsaren. Om du inte har något BankID kan du hämta ett hos din internetbank. Om du har ett BankID på en annan enhet kan du starta din BankID-app där.", "Searching for BankID:s, it may take a little while… If a few seconds have passed and still no BankID has been found, you probably don’t have a BankID which can be used for this identification/signing on this computer. If you have a BankID card, please insert it into your card reader. If you don’t have a BankID you can order one from your internet bank. If you have a BankID on another device you can start the BankID app on that device.") },
        { MessageShortName.RFA14B, new("Söker efter BankID, det kan ta en liten stund… Om det har gått några sekunder och inget BankID har hittats har du sannolikt inget BankID som går att använda för den aktuella identifieringen/underskriften i den här enheten. Om du inte har något BankID kan du hämta ett hos din internetbank. Om du har ett BankID på en annan enhet kan du starta din BankID-app där.", "Searching for BankID:s, it may take a little while… If a few seconds have passed and still no BankID has been found, you probably don’t have a BankID which can be used for this identification/signing on this device. If you don’t have a BankID you can order one from your internet bank. If you have a BankID on another device you can start the BankID app on that device.") },
        { MessageShortName.RFA15A, new("Söker efter BankID, det kan ta en liten stund… Om det har gått några sekunder och inget BankID har hittats har du sannolikt inget BankID som går att använda för den aktuella identifieringen/underskriften i den här datorn. Om du har ett BankID-kort, sätt in det i kortläsaren. Om du inte har något BankID kan du hämta ett hos din internetbank.", "Searching for BankID:s, it may take a little while… If a few seconds have passed and still no BankID has been found, you probably don’t have a BankID which can be used for this identification/signing on this computer. If you have a BankID card, please insert it into your card reader. If you don’t have a BankID you can order one from your internet bank.") },
        { MessageShortName.RFA15B, new("Söker efter BankID, det kan ta en liten stund… Om det har gått några sekunder och inget BankID har hittats har du sannolikt inget BankID som går att använda för den aktuella identifieringen/underskriften i den här enheten. Om du inte har något BankID kan du hämta ett hos din internetbank.", "Searching for BankID:s, it may take a little while… If a few seconds have passed and still no BankID has been found, you probably don’t have a BankID which can be used for this identification/signing on this device. If you don’t have a BankID you can order one from your internet bank.") },
        { MessageShortName.RFA16, new("Det BankID du försöker använda är för gammalt eller spärrat. Använd ett annat BankID eller hämta ett nytt hos din internetbank.", "The BankID you are trying to use is revoked or too old. Please use another BankID or order a new one from your internet bank.") },
        { MessageShortName.RFA17A, new("BankID-appen verkar inte finnas i din dator eller telefon. Installera den och hämta ett BankID hos din internetbank. Installera appen från din appbutik eller https://install.bankid.com.", "The BankID app couldn’t be found on your computer or mobile device. Please install it and order a BankID from your internet bank. Install the app from your app store or https://install.bankid.com.") },
        { MessageShortName.RFA17B, new("Misslyckades att läsa av QR-koden. Starta BankID-appen och läs av QR-koden. Om du inte har BankID-appen måste du installera den och hämta ett BankID hos din internetbank. Installera appen från din appbutik eller https://install.bankid.com.", "Failed to scan the QR code. Start the BankID app and scan the QR code. If you don't have the BankID app, you need to install it and order a BankID from your internet bank. Install the app from your app store or https://install.bankid.com.") },
        { MessageShortName.RFA18, new("Starta BankID-appen", "Start the BankID app") },
        { MessageShortName.RFA19, new("Vill du identifiera dig eller skriva under med BankID på den här datorn eller med ett Mobilt BankID?", "Would you like to identify yourself or sign with a BankID on this computer or with a Mobile BankID?") },
        { MessageShortName.RFA2, new("Du har inte BankID-appen installerad. Kontakta din internetbank.", "The BankID app is not installed. Please contact your internet bank.") },
        { MessageShortName.RFA20, new("Vill du identifiera dig eller skriva under med ett BankID på den här enheten eller med ett BankID på en annan enhet?", "Would you like to identify yourself or sign with a BankID on this device or with a BankID on another device?") },
        { MessageShortName.RFA21, new("Identifiering eller underskrift pågår.", "Identification or signing in progress.") },
        { MessageShortName.RFA22, new("Okänt fel. Försök igen.", "Unknown error. Please try again.") },
        { MessageShortName.RFA3, new("Åtgärden avbruten. Försök igen.", "Action cancelled. Please try again.") },
        { MessageShortName.RFA4, new("En identifiering eller underskrift för det här personnumret är redan påbörjad. Försök igen.", "An identification or signing for this personal number is already started. Please try again.") },
        { MessageShortName.RFA5, new("Internt tekniskt fel. Försök igen.", "Internal error. Please try again.") },
        { MessageShortName.RFA6, new("Åtgärden avbruten.", "Action cancelled.") },
        { MessageShortName.RFA8, new("BankID-appen svarar inte. Kontrollera att den är startad och att du har internetanslutning. Om du inte har något giltigt BankID kan du hämta ett hos din Bank. Försök sedan igen.", "The BankID app is not responding. Please check that the program is started and that you have internet access. If you don’t have a valid BankID you can get one from your bank. Try again.") },
        { MessageShortName.RFA9, new("Skriv in din säkerhetskod i BankID-appen och välj Legitimera eller Skriv under.", "Enter your security code in the BankID app and select Identify or Sign.") },
    };

    public string GetLocalizedString(MessageShortName messageShortName)
    {
        if(_translations.TryGetValue(messageShortName, out var translation))
        {
            var language = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            if (language.Equals(SwedishTwoLetterIsoLanguageName, StringComparison.InvariantCultureIgnoreCase))
            {
                return translation.Swedish;
            }
            else
            {
                return translation.English;
            }
        }

        return messageShortName.ToString();
    }

    private readonly record struct Translation(string Swedish, string English);

}
