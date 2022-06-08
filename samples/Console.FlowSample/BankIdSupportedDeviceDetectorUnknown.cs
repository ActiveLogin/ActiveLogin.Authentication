using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

public class BankIdSupportedDeviceDetectorUnknown : IBankIdSupportedDeviceDetector
{
    public BankIdSupportedDevice Detect()
    {
        return BankIdSupportedDevice.Unknown;
    }
}
