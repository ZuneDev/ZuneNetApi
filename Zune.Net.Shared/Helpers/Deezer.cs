using Atom.Xml;

namespace Zune.Net.Helpers
{
    public static partial class Deezer
    {
        public const string API_BASE = "https://api.deezer.com";

        public static readonly Author DZ_AUTHOR = new()
        {
            Name = "Deezer",
            Url = "https://deezer.com"
        };
    }
}
