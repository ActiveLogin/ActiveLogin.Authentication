using ActiveLogin.Authentication.BankId.Core.Launcher;

namespace ActiveLogin.Authentication.BankId.Core.Flow;

public class BankIdFlowInitializeAuthLaunchTypeSameDevice : BankIdFlowInitializeAuthLaunchType
{
    public BankIdLaunchInfo BankIdLaunchInfo { get; init; }

    public BankIdFlowInitializeAuthLaunchTypeSameDevice(BankIdLaunchInfo bankIdLaunchInfo)
    {
        BankIdLaunchInfo = bankIdLaunchInfo;
    }
}
