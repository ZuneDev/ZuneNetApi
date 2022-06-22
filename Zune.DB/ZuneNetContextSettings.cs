namespace Zune.DB
{
    public class ZuneNetContextSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string MemberCollectionName { get; set; } = "Members";

        public string AuthCollectionName { get; set; } = "Auth";
    }
}
