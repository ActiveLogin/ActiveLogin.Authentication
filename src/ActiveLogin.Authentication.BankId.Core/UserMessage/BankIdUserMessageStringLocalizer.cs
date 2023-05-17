using ActiveLogin.Authentication.BankId.Api.UserMessage;

namespace ActiveLogin.Authentication.BankId.Core.UserMessage;

public class BankIdUserMessageLocalizer : IBankIdUserMessageLocalizer
{
    private const string SwedishTwoLetterIsoLanguageName = "sv";

    private static readonly Dictionary<MessageShortName, Translation> Translations = new()
    {
        { MessageShortName.RFA1, new("Starta BankID-appen", "Start your BankID app.") },
        { MessageShortName.RFA1QR, new("Starta BankID-appen och scanna QR-koden.", "Start your BankID app and scan the QR code.") },

        { MessageShortName.RFA2, new("Du har inte BankID-appen installerad. Kontakta din bank.", "The BankID app is not installed. Please contact your bank.") },
        { MessageShortName.RFA3, new("Åtgärden avbruten. Försök igen.", "The action was cancelled. Please try again") },
        { MessageShortName.RFA4, new("En identifiering eller underskrift för det här personnumret är redan påbörjad. Försök igen.", "An identification or signing for this personal number is already started. Please try again.") },
        { MessageShortName.RFA5, new("Något gick fel. Försök igen.", "Something went wrong. Please try again") },
        { MessageShortName.RFA6, new("Åtgärden avbröts.", "The action was cancelled.") },
        { MessageShortName.RFA8, new("BankID-appen svarar inte. Kontrollera att den är startad och att du har internetanslutning. Om du inte har ett giltigt BankID kan du skaffa ett hos din bank. Försök sedan igen.", "The BankID app is not responding. Please check that it’s started and that you have internet access. If you don’t have a valid BankID you can get one from your bank. Try again.") },
        { MessageShortName.RFA9, new("Skriv in din säkerhetskod i BankID- appen och välj Identifiera eller Skriv under.", "Enter your security code in the BankID app and select Identify or Sign.") },

        { MessageShortName.RFA13, new("Försöker starta BankID-appen.", "Trying to start your BankID app.") },

        { MessageShortName.RFA14A, new("Söker efter BankID, det kan ta en liten stund… Om det har gått flera sekunder och inget BankID har hittats har du sannolikt inget BankID som går att använda för den aktuella identifieringen/underskriften i den här datorn. Om du har ett BankID-kort, sätt in det i kortläsaren. Om du inte har något BankID kan du skaffa ett hos din bank. Om du har ett BankID på en annan enhet kan du starta din BankID-app där.", "Searching for BankID, it may take a little while… If several seconds have passed and no BankID has been found, you probably don’t have a BankID which can be used for this identification/signing on this computer. If you have a BankID card, please insert it into your card reader. If you don’t have a BankID you can get one from your bank. If you have a BankID on another device you can start the BankID app on that device.") },
        { MessageShortName.RFA14B, new("Söker efter BankID, det kan ta en liten stund… Om det har gått flera sekunder och inget BankID har hittats har du sannolikt inget BankID som går att använda för den aktuella identifieringen/underskriften i den här enheten. Om du inte har något BankID kan du hämta ett hos din bank. Om du har ett BankID på en annan enhet kan du starta din BankID-app där.", "Searching for BankID, it may take a little while… If several seconds have passed and still no BankID has been found, you probably don’t have a BankID which can be used for this identification/signing on this device. If you don’t have a BankID you can get one from your bank. If you have a BankID on another device you can start the BankID app on that device.") },

        { MessageShortName.RFA15A, new("Söker efter BankID, det kan ta en liten stund… Om det har gått flera sekunder och inget BankID har hittats har du sannolikt inget BankID som går att använda för den aktuella identifieringen/underskriften i den här datorn. Om du har ett BankID-kort, sätt in det i kortläsaren. Om du inte har något BankID kan du skaffa ett hos din bank.", "Searching for BankID, it may take a little while… If several seconds have passed and still no BankID has been found, you probably don’t have a BankID which can be used for this identification/signing on this computer. If you have a BankID card, please insert it into your card reader. If you don’t have a BankID you can get one from your bank.") },
        { MessageShortName.RFA15B, new("Söker efter BankID, det kan ta en liten stund… Om det har gått flera sekunder och inget BankID har hittats har du sannolikt inget BankID som går att använda för den aktuella identifieringen/underskriften i den här enheten. Om du inte har något BankID kan du skaffa ett hos din bank.", "Searching for BankID, it may take a little while… If several seconds have passed and still no BankID has been found, you probably don’t have a BankID which can be used for this identification/signing on this device. If you don’t have a BankID you can get one from your bank.") },

        { MessageShortName.RFA16, new("Det BankID du försöker använda är för gammalt eller spärrat. Använd ett annat BankID eller skaffa ett nytt hos din bank.", "The BankID you are trying to use is blocked or too old. Please use another BankID or get a new one from your bank.") },

        { MessageShortName.RFA17A, new("BankID-appen verkar inte finnas i din dator eller mobil. Installera den och skaffa ett BankID hos din bank. Installera appen från din appbutik eller https://install.bankid.com.", "The BankID app couldn’t be found on your computer or mobile device. Please install it and get a BankID from your bank. Install the app from your app store or https://install.bankid.com.") },
        { MessageShortName.RFA17B, new("Misslyckades att läsa av QR-koden. Starta BankID-appen och läs av QRkoden. Kontrollera att BankID-appen är uppdaterad. Om du inte har appen måste du installera den och skaffa ett BankID hos din bank. Installera appen från din appbutik eller https://install.bankid.com.", "Failed to scan the QR code. Start the BankID app and scan the QR code. Check that the BankID app is up to date. If you don't have the app, you need to install it and get a BankID from your bank. Install the app from your app store or https://install.bankid.com.") },

        { MessageShortName.RFA18, new("Starta BankID-appen", "Start the BankID app") },
        { MessageShortName.RFA19, new("Vill du identifiera dig eller skriva under med BankID på den här datorn eller med ett Mobilt BankID?", "Would you like to identify yourself or sign with a BankID on this computer or with a Mobile BankID?") },
        { MessageShortName.RFA20, new("Vill du identifiera dig eller skriva under med ett BankID på den här enheten eller på en annan enhet?", "Would you like to identify yourself or sign with a BankID on this device, or on another device?") },
        { MessageShortName.RFA21, new("Identifiering eller underskrift pågår.", "Identification or signing in progress.") },
        { MessageShortName.RFA22, new("Okänt fel. Försök igen.", "Unknown error. Please try again.") },
        { MessageShortName.RFA23, new("Fotografera och läs av din ID-handling med BankID-appen.", "Using your BankID app, take a photo of, and scan, your ID document.") }
    };

    public string GetLocalizedString(MessageShortName messageShortName)
    {
        if(Translations.TryGetValue(messageShortName, out var translation))
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
