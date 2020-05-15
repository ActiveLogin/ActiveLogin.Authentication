namespace ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice
{
    public class BankIdSupportedDeviceOsVersion
    {
        public BankIdSupportedDeviceOsVersion(int? majorVersion = null, int? minorVersion = null, int? patch = null)
        {
            MajorVersion = majorVersion;
            MinorVersion = minorVersion;
            Patch = patch;
        }

        public int? MajorVersion { get; }
        public int? MinorVersion { get; }
        public int? Patch { get;}
    }
}
