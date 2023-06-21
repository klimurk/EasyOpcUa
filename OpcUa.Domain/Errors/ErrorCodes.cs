using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUa.Domain.Errors;

public enum ErrorCodes
{

    ServersNotFoundCode,
    EndpointSecurityCodeNotFoundCode,
    EndpointsNotFoundCode,
    ConnectServerErrorCode,
    CertificateValidationErrorCode,
    ReferenceHasInnerErrorCode,
    BrowseNodeCode,
    BrowseRootCode,
    ReadNodeCode,
    CreateSessionSubscriptionErrorCode,
    RemoveSubscriptionErrorCode,
    SubscriptionExist,
    SubscribeError,
}
