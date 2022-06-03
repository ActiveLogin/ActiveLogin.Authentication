namespace Standalone.MvcSample.Models;

public class SignResultViewModel
{
    public SignResultViewModel(int hashCodeOfSignedDocument, string fileName, string fileType, string personalIdentityNumber, string name, string ipAddress, string signature)
    {
        HashCodeOfSignedDocument = hashCodeOfSignedDocument;
        FileName = fileName;
        FileType = fileType;
        PersonalIdentityNumber = personalIdentityNumber;
        Name = name;
        IpAddress = ipAddress;
        SignatureXml = signature;
    }

    public int HashCodeOfSignedDocument { get; }
    public string FileName { get; }
    public string FileType { get; }
    public string PersonalIdentityNumber { get; }
    public string Name { get; }
    public string IpAddress { get; }
    public string SignatureXml { get; }
}
