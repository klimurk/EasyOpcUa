using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUa.Applications.Errors;

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
}
