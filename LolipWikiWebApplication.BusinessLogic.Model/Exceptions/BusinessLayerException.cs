using System;
using System.Diagnostics.CodeAnalysis;

namespace LolipWikiWebApplication.BusinessLogic.Model.Exceptions
{
    [SuppressMessage("Design", "RCS1194:Implement exception constructors.", Justification = "<Pending>")]
    public class BusinessLayerException : Exception
    {
        public BusinessLayerException(BusinessLayerErrorCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public BusinessLayerException(BusinessLayerErrorCode errorCode, string message, Exception innerException) : base(innerException.Message, innerException)
        {
            ErrorCode = errorCode;
        }

        public BusinessLayerErrorCode ErrorCode { get; }
    }
}