namespace LolipWikiWebApplication.BusinessLogic.Model
{
    public enum BusinessLayerErrorCode
    {
        None,
        Unauthorized,
        NotFound,
        ApiException,
        UserIsLocked,
        ExistingDraft,
        UserRoleMissing,
        UserSubMissing
    }
}