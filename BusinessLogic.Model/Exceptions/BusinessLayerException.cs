using System;
using System.Diagnostics.CodeAnalysis;

namespace LolipWikiWebApplication.BusinessLogic.Model.Exceptions
{
    [SuppressMessage("Design", "RCS1194:Implement exception constructors.", Justification = "<Pending>")]
    public abstract class BusinessLayerException : Exception
    {
        protected BusinessLayerException(BusinessLayerErrorCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        protected BusinessLayerException(BusinessLayerErrorCode errorCode, string message, Exception innerException) : base(innerException.Message, innerException)
        {
            ErrorCode = errorCode;
        }

        public BusinessLayerErrorCode ErrorCode { get; }
    }
}