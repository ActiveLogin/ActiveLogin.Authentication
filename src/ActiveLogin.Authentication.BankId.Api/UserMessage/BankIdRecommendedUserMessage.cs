using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api.UserMessage;

public class BankIdRecommendedUserMessage : IBankIdUserMessage
{
    private static readonly List<CollectResponseMapping> CollectResponseMappings = new()
    {
        new (MessageShortName.RFA1QR, new [] { CollectStatus.Pending }, new [] { CollectHintCode.NoClient }, tokenStartRequired: true, usingQrCode: true),
        new (MessageShortName.RFA1, new [] { CollectStatus.Pending }, new [] { CollectHintCode.NoClient }, tokenStartRequired: true, usingQrCode: false),
        new (MessageShortName.RFA13, CollectStatus.Pending, CollectHintCode.NoClient, tokenStartRequired: false),

        new (MessageShortName.RFA1QR, new [] { CollectStatus.Pending }, new [] { CollectHintCode.OutstandingTransaction, CollectHintCode.NoClient }, tokenStartRequired: true, usingQrCode: true),
        new (MessageShortName.RFA1, new [] { CollectStatus.Pending }, new [] { CollectHintCode.OutstandingTransaction, CollectHintCode.NoClient }, tokenStartRequired: true, usingQrCode: false),
        new (MessageShortName.RFA13, new [] { CollectStatus.Pending }, new [] { CollectHintCode.OutstandingTransaction, CollectHintCode.NoClient }, tokenStartRequired: false),

        new (MessageShortName.RFA9, CollectStatus.Pending, CollectHintCode.UserSign),
        new (MessageShortName.RFA14A, CollectStatus.Pending, CollectHintCode.Started, tokenStartRequired: true, accessedFromMobileDevice: false),
        new (MessageShortName.RFA14B, CollectStatus.Pending, CollectHintCode.Started, tokenStartRequired: true, accessedFromMobileDevice: true),
        new (MessageShortName.RFA15A, CollectStatus.Pending, CollectHintCode.Started, tokenStartRequired: false, accessedFromMobileDevice: false),
        new (MessageShortName.RFA15B, CollectStatus.Pending, CollectHintCode.Started, tokenStartRequired: false, accessedFromMobileDevice: true),


        new (MessageShortName.RFA3, CollectHintCode.Cancelled),
        new (MessageShortName.RFA6, CollectHintCode.UserCancel),
        new (MessageShortName.RFA8, CollectHintCode.ExpiredTransaction),
        new (MessageShortName.RFA16, CollectHintCode.CertificateErr),
        new (MessageShortName.RFA17A, CollectHintCode.StartFailed, usingQrCode: false),
        new (MessageShortName.RFA17B, CollectHintCode.StartFailed, usingQrCode: true),

        new (MessageShortName.RFA21, CollectStatus.Pending, CollectHintCode.Unknown),
        new (MessageShortName.RFA22, CollectStatus.Failed, CollectHintCode.Unknown),

        new (MessageShortName.RFA23, CollectStatus.Pending, CollectHintCode.userMrtd)
    };

    private static readonly List<ErrorResponseMapping> ErrorResponseMappings = new List<ErrorResponseMapping>()
    {
        new (MessageShortName.RFA3, ErrorCode.Canceled),
        new (MessageShortName.RFA4, ErrorCode.AlreadyInProgress),
        new (MessageShortName.RFA5, ErrorCode.RequestTimeout, ErrorCode.Maintenance, ErrorCode.InternalError)
    };

    public MessageShortName GetMessageShortNameForCollectResponse(
        CollectStatus collectStatus,
        CollectHintCode hintCode,
        bool tokenStartRequired,
        bool accessedFromMobileDevice,
        bool usingQrCode)
    {
        var mapping = CollectResponseMappings
            .Where(x => !x.CollectStatuses.Any() || x.CollectStatuses.Contains(collectStatus))
            .Where(x => !x.CollectHintCodes.Any() || x.CollectHintCodes.Contains(hintCode))
            .Where(x => x.TokenStartRequired == null || x.TokenStartRequired == tokenStartRequired)
            .Where(x => x.AccessedFromMobileDevice == null || x.AccessedFromMobileDevice == accessedFromMobileDevice)
            .Where(x => x.UsingQrCode == null || x.UsingQrCode == usingQrCode)
            .FirstOrDefault(x => x.MessageShortName != MessageShortName.Unknown);

        return mapping?.MessageShortName ?? MessageShortName.RFA22;
    }

    public MessageShortName GetMessageShortNameForErrorResponse(ErrorCode errorCode)
    {
        var mapping = ErrorResponseMappings
            .Where(x => !x.ErrorCodes.Any() || x.ErrorCodes.Contains(errorCode))
            .FirstOrDefault(x => x.MessageShortName != MessageShortName.Unknown);

        return mapping?.MessageShortName ?? MessageShortName.RFA22;
    }

    private class CollectResponseMapping
    {
        public MessageShortName MessageShortName { get; }

        public List<CollectHintCode> CollectHintCodes { get; } = new List<CollectHintCode>();
        public List<CollectStatus> CollectStatuses { get; } = new List<CollectStatus>();
        public bool? TokenStartRequired { get; }
        public bool? AccessedFromMobileDevice { get; }
        public bool? UsingQrCode { get; }

        public CollectResponseMapping(MessageShortName messageShortName, CollectStatus collectStatus, CollectHintCode collectHintCode, bool? tokenStartRequired = null, bool? accessedFromMobileDevice = null)
            : this(messageShortName, new List<CollectStatus>() { collectStatus }, new List<CollectHintCode>() { collectHintCode }, tokenStartRequired, accessedFromMobileDevice)
        {
        }

        public CollectResponseMapping(MessageShortName messageShortName, params CollectHintCode[] collectHintCodes)
            : this(messageShortName, new List<CollectStatus>() { }, collectHintCodes.ToList())
        {
        }

        public CollectResponseMapping(MessageShortName messageShortName, CollectHintCode collectHintCode, bool usingQrCode)
            : this(messageShortName, new List<CollectStatus>() { }, new List<CollectHintCode>() { collectHintCode }, null, null, usingQrCode)
        {
        }

        public CollectResponseMapping(MessageShortName messageShortName, CollectStatus collectStatus, params CollectHintCode[] collectHintCodes)
            : this(messageShortName, new List<CollectStatus>() { collectStatus }, collectHintCodes.ToList())
        {
        }

        public CollectResponseMapping(
            MessageShortName messageShortName,
            IEnumerable<CollectStatus> collectStatuses,
            IEnumerable<CollectHintCode> collectHintCodes,
            bool? tokenStartRequired = null,
            bool? accessedFromMobileDevice = null,
            bool? usingQrCode = null)
        {
            MessageShortName = messageShortName;
            CollectStatuses.AddRange(collectStatuses);
            CollectHintCodes.AddRange(collectHintCodes);
            TokenStartRequired = tokenStartRequired;
            AccessedFromMobileDevice = accessedFromMobileDevice;
            UsingQrCode = usingQrCode;
        }
    }

    private class ErrorResponseMapping
    {
        public MessageShortName MessageShortName { get; }

        public List<ErrorCode> ErrorCodes { get; } = new List<ErrorCode>();

        public ErrorResponseMapping(MessageShortName messageShortName, params ErrorCode[] errorCodes)
            : this(messageShortName, errorCodes.ToList())
        {
        }

        private ErrorResponseMapping(MessageShortName messageShortName, IEnumerable<ErrorCode> errorCodes)
        {
            MessageShortName = messageShortName;
            ErrorCodes.AddRange(errorCodes);
        }
    }
}
