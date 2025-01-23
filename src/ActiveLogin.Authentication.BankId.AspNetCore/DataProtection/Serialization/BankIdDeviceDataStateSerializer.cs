using ActiveLogin.Authentication.BankId.AspNetCore.UserContext.Device.State;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;
internal class BankIdDeviceDataStateSerializer : BankIdDataSerializer<DeviceDataState>
{
    protected override void Write(BinaryWriter writer, DeviceDataState model)
    {
        writer.Write(model.DeviceIdentifier);
    }

    protected override DeviceDataState Read(BinaryReader reader)
    {
        return new DeviceDataState(reader.ReadString());
    }
}
