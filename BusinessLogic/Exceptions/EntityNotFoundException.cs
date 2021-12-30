using LolipWikiWebApplication.BusinessLogic.Model;
using LolipWikiWebApplication.BusinessLogic.Model.Exceptions;

namespace LolipWikiWebApplication.BusinessLogic.Exceptions
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1194:Implement exception constructors.", Justification = "<Pending>")]
    public class EntityNotFoundException<TEntity> : BusinessLayerException
    {
        public EntityNotFoundException(long id) : base(BusinessLayerErrorCode.NotFound, $"Not Entity of Type:{typeof(TEntity).Name} was found with Id:{id}")
        {
        }
    }
}