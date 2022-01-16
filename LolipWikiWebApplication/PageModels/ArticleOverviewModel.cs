using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;

namespace LolipWikiWebApplication.PageModels
{
    public class ArticleOverviewModel
    {
        public IUser User { get; }

        public ArticleOverviewModel(IUser user)
        {
            User = user;
        }
    }
}