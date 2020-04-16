using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for successful initiation of authentication order. 
    /// </summary>
    public class BankIdAuthSuccessEvent : BankIdEvent
    {
        public BankIdAuthSuccessEvent(SwedishPersonalIdentityNumber? personalIdentityNumber, string orderRef)
            : base(BankIdEventTypeIds.BankIdAuthSuccess, BankIdEventTypeNames.BankIdAuthSuccess, EventSeverity.Success)
        {
            PersonalIdentityNumber = personalIdentityNumber;
            OrderRef = orderRef;
        }

        public SwedishPersonalIdentityNumber? PersonalIdentityNumber { get; set; }

        public string OrderRef { get; set; }
    }
}

