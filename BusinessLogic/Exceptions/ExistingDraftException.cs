using LolipWikiWebApplication.BusinessLogic.Model;
using LolipWikiWebApplication.BusinessLogic.Model.Exceptions;

namespace LolipWikiWebApplication.BusinessLogic.Exceptions
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1194:Implement exception constructors.", Justification = "<Pending>")]
    public class ExistingDraftException : BusinessLayerException
    {
        public ExistingDraftException(long articleId, long existingArticleVersionId, long userId) : base(BusinessLayerErrorCode.ExistingDraft, $"The Article: {articleId} already has a Draft Version:{existingArticleVersionId} from User:{userId}")
        {
        }
    }
}