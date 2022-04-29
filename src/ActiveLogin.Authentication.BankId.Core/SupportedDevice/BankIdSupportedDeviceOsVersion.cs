namespace ActiveLogin.Authentication.BankId.Core.SupportedDevice;

public class BankIdSupportedDeviceOsVersion
{
    public BankIdSupportedDeviceOsVersion(int majorVersion)
    {
        MajorVersion = majorVersion;
    }

    public BankIdSupportedDeviceOsVersion(int majorVersion, int minorVersion)
        : this(majorVersion)
    {
        MinorVersion = minorVersion;
    }

    public BankIdSupportedDeviceOsVersion(int majorVersion, int minorVersion, int patch)
        : this(majorVersion, minorVersion)
    {
        Patch = patch;
    }

    private BankIdSupportedDeviceOsVersion()
    {
    }

    public static BankIdSupportedDeviceOsVersion Empty = new BankIdSupportedDeviceOsVersion();

    public int? MajorVersion { get; }
    public int? MinorVersion { get; }
    public int? Patch { get; }

    public override string ToString()
    {
        return $"{MajorVersion}.{MinorVersion}.{Patch}".TrimEnd('.');
    }
}
