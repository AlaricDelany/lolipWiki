namespace LolipWikiWebApplication.BusinessLogic.Providers
{
    public class ProvideCurrentVersion : IProvideCurrentVersion
    {
        public string Version
            => GetType()
               .Assembly.GetName()
               .Version?.ToString()
            ?? string.Empty;
    }
}