using System.Collections.Generic;
using System.Linq;
using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api.UserMessage
{
    public class BankIdRecommendedUserMessage : IBankIdUserMessage
    {
        private static readonly List<CollectResponseMapping> CollectResponseMappings = new List<CollectResponseMapping>
        {
            new CollectResponseMapping(MessageShortName.RFA1, CollectStatus.Pending, CollectHintCode.NoClient, true),
            new CollectResponseMapping(MessageShortName.RFA13, CollectStatus.Pending, CollectHintCode.NoClient, false),

            new CollectResponseMapping(MessageShortName.RFA1, new[] { CollectStatus.Pending }, new[] { CollectHintCode.OutstandingTransaction, CollectHintCode.NoClient }, true),
            new CollectResponseMapping(MessageShortName.RFA13, new[] { CollectStatus.Pending }, new[] { CollectHintCode.OutstandingTransaction, CollectHintCode.NoClient }, false),

            new CollectResponseMapping(MessageShortName.RFA9, CollectStatus.Pending, CollectHintCode.UserSign),
            new CollectResponseMapping(MessageShortName.RFA14A, CollectStatus.Pending, CollectHintCode.Started, true, false),
            new CollectResponseMapping(MessageShortName.RFA14B, CollectStatus.Pending, CollectHintCode.Started, true, true),
            new CollectResponseMapping(MessageShortName.RFA15A, CollectStatus.Pending, CollectHintCode.Started, false, false),
            new CollectResponseMapping(MessageShortName.RFA15B, CollectStatus.Pending, CollectHintCode.Started, false, true),


            new CollectResponseMapping(MessageShortName.RFA3, CollectHintCode.Cancelled),
            new CollectResponseMapping(MessageShortName.RFA6, CollectHintCode.UserCancel),
            new CollectResponseMapping(MessageShortName.RFA8, CollectHintCode.ExpiredTransaction),
            new CollectResponseMapping(MessageShortName.RFA16, CollectHintCode.CertificateErr),
            new CollectResponseMapping(MessageShortName.RFA17, CollectHintCode.StartFailed),

            new CollectResponseMapping(MessageShortName.RFA21, CollectStatus.Pending, CollectHintCode.Unknown),
            new CollectResponseMapping(MessageShortName.RFA22, CollectStatus.Failed, CollectHintCode.Unknown)
        };

        private static readonly List<ErrorResponseMapping> ErrorResponseMappings = new List<ErrorResponseMapping>
        {
            new ErrorResponseMapping(MessageShortName.RFA3, ErrorCode.Canceled),
            new ErrorResponseMapping(MessageShortName.RFA4, ErrorCode.AlreadyInProgress),
            new ErrorResponseMapping(MessageShortName.RFA5, ErrorCode.RequestTimeout, ErrorCode.Maintenance, ErrorCode.InternalError)
        };

        public MessageShortName GetMessageShortNameForCollectResponse(CollectStatus collectStatus,
            CollectHintCode hintCode, bool authPersonalIdentityNumberProvided, bool accessedFromMobileDevice)
        {
            CollectResponseMapping mapping = CollectResponseMappings
                .Where(x => !x.CollectStatuses.Any() || x.CollectStatuses.Contains(collectStatus))
                .Where(x => !x.CollectHintCodes.Any() || x.CollectHintCodes.Contains(hintCode))
                .Where(x => x.AuthPersonalIdentityNumberProvided == null || x.AuthPersonalIdentityNumberProvided == authPersonalIdentityNumberProvided)
                .Where(x => x.AccessedFromMobileDevice == null || x.AccessedFromMobileDevice == accessedFromMobileDevice)
                .FirstOrDefault(x => x.MessageShortName != MessageShortName.Unknown);

            return mapping?.MessageShortName ?? MessageShortName.RFA22;
        }

        public MessageShortName GetMessageShortNameForErrorResponse(ErrorCode errorCode)
        {
            ErrorResponseMapping mapping = ErrorResponseMappings
                .Where(x => !x.ErrorCodes.Any() || x.ErrorCodes.Contains(errorCode))
                .FirstOrDefault(x => x.MessageShortName != MessageShortName.Unknown);

            return mapping?.MessageShortName ?? MessageShortName.RFA22;
        }

        private class CollectResponseMapping
        {
            public CollectResponseMapping(MessageShortName messageShortName, params CollectHintCode[] collectHintCodes)
                : this(messageShortName, new List<CollectStatus>(), collectHintCodes.ToList())
            {
            }

            public CollectResponseMapping(MessageShortName messageShortName, CollectStatus collectStatus, CollectHintCode collectHintCode, bool? authPersonalIdentityNumberProvided = null, bool? accessedFromMobileDevice = null)
                : this(messageShortName, new List<CollectStatus> { collectStatus }, new List<CollectHintCode> { collectHintCode }, authPersonalIdentityNumberProvided, accessedFromMobileDevice)
            {
            }

            public CollectResponseMapping(MessageShortName messageShortName, CollectStatus collectStatus, params CollectHintCode[] collectHintCodes)
                : this(messageShortName, new List<CollectStatus> { collectStatus }, collectHintCodes.ToList())
            {
            }

            public CollectResponseMapping(
                MessageShortName messageShortName,
                IEnumerable<CollectStatus> collectStatuses,
                IEnumerable<CollectHintCode> collectHintCodes,
                bool? authPersonalIdentityNumberProvided = null,
                bool? accessedFromMobileDevice = null
            ){
                MessageShortName = messageShortName;
                CollectStatuses.AddRange(collectStatuses);
                CollectHintCodes.AddRange(collectHintCodes);
                AuthPersonalIdentityNumberProvided = authPersonalIdentityNumberProvided;
                AccessedFromMobileDevice = accessedFromMobileDevice;
            }

            public MessageShortName MessageShortName { get; }

            public List<CollectHintCode> CollectHintCodes { get; } = new List<CollectHintCode>();
            public List<CollectStatus> CollectStatuses { get; } = new List<CollectStatus>();
            public bool? AuthPersonalIdentityNumberProvided { get; }
            public bool? AccessedFromMobileDevice { get; }
        }

        private class ErrorResponseMapping
        {
            public ErrorResponseMapping(MessageShortName messageShortName, params ErrorCode[] errorCodes)
                : this(messageShortName, errorCodes.ToList())
            {
            }

            private ErrorResponseMapping(MessageShortName messageShortName, IEnumerable<ErrorCode> errorCodes)
            {
                MessageShortName = messageShortName;
                ErrorCodes.AddRange(errorCodes);
            }

            public MessageShortName MessageShortName { get; }

            public List<ErrorCode> ErrorCodes { get; } = new List<ErrorCode>();
        }
    }
}
