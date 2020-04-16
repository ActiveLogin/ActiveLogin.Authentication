namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    public interface IBankIdEventTrigger
    {
        void TriggerAsync(BankIdEvent bankIdEvent);
    }
}
