using ActiveLogin.Authentication.BankId.Core.Launcher;

namespace ActiveLogin.Authentication.BankId.Core.Flow;

public class BankIdFlowInitializeLaunchTypeSameDevice : BankIdFlowInitializeLaunchType
{
    public BankIdLaunchInfo BankIdLaunchInfo { get; init; }

    public BankIdFlowInitializeLaunchTypeSameDevice(BankIdLaunchInfo bankIdLaunchInfo)
    {
        BankIdLaunchInfo = bankIdLaunchInfo;
    }
}
