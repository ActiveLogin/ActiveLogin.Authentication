using ActiveLogin.Authentication.BankId.AspNetCore.Launcher;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Flow;

public class BankIdFlowInitializeAuthLaunchTypeSameDevice : BankIdFlowInitializeAuthLaunchType
{
    public BankIdLaunchInfo BankIdLaunchInfo { get; init; }

    public BankIdFlowInitializeAuthLaunchTypeSameDevice(BankIdLaunchInfo bankIdLaunchInfo)
    {
        BankIdLaunchInfo = bankIdLaunchInfo;
    }
}
