using ActiveLogin.Authentication.BankId.Api.UserMessage;

namespace ActiveLogin.Authentication.BankId.Core.UserMessage;

public class BankIdUserMessageLocalizer : IBankIdUserMessageLocalizer
{
    private const string SwedishTwoLetterIsoLanguageName = "sv";

    private static readonly Dictionary<MessageShortName, Translation> Translations = new()
    {
        { MessageShortName.RFA1, new("Starta BankID-appen",
                                     "Start your BankID app.") },

        { MessageShortName.RFA2, new("Du har inte BankID-appen installerad. Kontakta din bank.",
                                     "The BankID app is not installed. Please contact your bank.") },

        { MessageShortName.RFA3, new("Åtgärden avbruten. Försök igen.",
                                     "The action was cancelled. Please try again") },

        { MessageShortName.RFA4, new("En identifiering eller underskrift för det här personnumret är redan påbörjad. Försök igen.",
                                      "An identification or signing for this personal number is already started. Please try again.") },

        { MessageShortName.RFA5, new("Något gick fel. Försök igen.",
                                     "Something went wrong. Please try again") },

        { MessageShortName.RFA6, new("Åtgärden avbröts.",
                                     "The action was cancelled.") },

        { MessageShortName.RFA8, new("BankID-appen svarar inte. Kontrollera att den är startad och att du har internetanslutning. Om du inte har ett giltigt BankID kan du skaffa ett hos din bank. Försök sedan igen.",
                                     "The BankID app is not responding. Please check that it’s started and that you have internet access. If you don’t have a valid BankID you can get one from your bank. Try again.") },

        { MessageShortName.RFA9, new("Skriv in din säkerhetskod i BankID- appen och välj Identifiera eller Skriv under.",
                                     "Enter your security code in the BankID app and select Identify or Sign.") },

        { MessageShortName.RFA13, new("Försöker starta BankID-appen.",
                                      "Trying to start your BankID app.") },

        { MessageShortName.RFA15A, new("Söker efter BankID. Säkerställ att du har ett giltigt BankID på den här datorn. Om du har ett BankID på kort, sätt in kortet i kortläsaren.",
                                       "Searching for BankID. Make sure you have a valid BankID on this computer. If you have a BankID on card, please insert the card into your card reader. ") },

        { MessageShortName.RFA15B, new("Söker efter BankID. Säkerställ att du har ett gitligt BankID på den här enheten.",
                                       "Searching for BankID. Make sure you have a valid BankID on this device.  ") },

        { MessageShortName.RFA16, new("Ditt BankID är för gammalt eller spärrat. Använd ett annat BankID eller skaffa ett nytt hos din bank.",
                                      "Your BankID is blocked or too old. Please use another BankID or get a new one from your bank.") },

        { MessageShortName.RFA17A, new("Du verkar inte ha BankID-appen/programmet. Installera den och skaffa ett BankID hos din bank.",
                                       "You don't seem to have the BankID app/program. Please install it and get a BankID from your bank.") },

        { MessageShortName.RFA17B, new("Misslyckades att läsa av QR-koden. Starta BankID-appen och läs av QR-koden.",
                                       "Failed to scan the QR code. Start the BankID app and scan the QR code.") },

        { MessageShortName.RFA19, new("Vill du använda BankID på den här datorn eller ett Mobilt BankID? ",
                                      "Would you like to use BankID on this computer, or a Mobile BankID? ") },

        { MessageShortName.RFA20, new("Vill du använda BankID på den här enheten eller på en annan enhet?",
                                      "Do you want to use BankID on this device or another device?") },

        { MessageShortName.RFA21, new("En identifiering eller underskrift pågår.",
                                      "An identification or signing is in progress. ") },

        { MessageShortName.RFA22, new("Något gick fel. Försök igen.",
                                      "Something went wrong. Please try again. ") },

        { MessageShortName.RFA23, new("Fotografera och läs av din ID-handling med BankID-appen.",
                                      "Take a photo of, and scan, you ID document with the BankID app.") }
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
