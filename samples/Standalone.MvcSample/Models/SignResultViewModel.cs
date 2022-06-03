namespace Standalone.MvcSample.Models;

public class SignResultViewModel
{
    public SignResultViewModel(int hashCodeOfSignedDocument, string personalIdentityNumber, string name, string ipAddress, string signature)
    {
        HashCodeOfSignedDocument = hashCodeOfSignedDocument;
        PersonalIdentityNumber = personalIdentityNumber;
        Name = name;
        IPAddress = ipAddress;
        SignatureXML = signature;
    }

    public int HashCodeOfSignedDocument { get; set; }
    public string PersonalIdentityNumber { get; set; }
    public string Name { get; set; }
    public string IPAddress { get; set; }
    public string SignatureXML { get; set; }
}
