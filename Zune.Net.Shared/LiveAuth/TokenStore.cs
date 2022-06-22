using System.Collections.Concurrent;

namespace Zune.Net.LiveAuth
{
    internal class TokenStore
    {
        private readonly ConcurrentDictionary<string, string> _tokenCidMapping = new();

        public static TokenStore Current { get; } = new();

        private TokenStore() { }

        public bool TryGetCidForToken(string token, out string cid)
            => _tokenCidMapping.TryGetValue(token, out cid);

        public void TryAddOrUpdateTokenForCid(string token, string cid)
            => _tokenCidMapping.AddOrUpdate(token, cid, (_, __) => cid);

        public void Clear() => _tokenCidMapping.Clear();
    }
}
