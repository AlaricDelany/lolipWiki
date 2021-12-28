namespace LolipWikiWebApplication.BusinessLogic.Model.UserManagement
{
    public record DefaultRequestor : IRequestor
    {
        internal DefaultRequestor()
        {
            Id                 = -1;
            Email              = string.Empty;
            ProfilePicturePath = string.Empty;
            DisplayName        = string.Empty;
            UserName           = string.Empty;
        }

        public long   Id                 { get; }
        public string Email              { get; }
        public string ProfilePicturePath { get; }
        public string UserName           { get; }
        public string DisplayName        { get; }
    }
}