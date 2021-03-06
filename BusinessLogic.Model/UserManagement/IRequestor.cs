namespace LolipWikiWebApplication.BusinessLogic.Model.UserManagement
{
    /// <summary>
    ///     Hold basic Information about the Caller of the Current Request that could be found in the pure Request
    /// </summary>
    public interface IRequestor
    {
        long   Id                 { get; }
        string Email              { get; }
        string ProfilePicturePath { get; }
        string UserName           { get; }
        string DisplayName        { get; }
    }
}