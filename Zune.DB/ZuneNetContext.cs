using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Zune.DB.Models;

namespace Zune.DB
{
    public class ZuneNetContext
    {
        private readonly IMongoCollection<Member> _memberCollection;
        private readonly IMongoCollection<TokenCidEntry> _authCollection;

        public ZuneNetContext(IOptions<ZuneNetContextSettings> dbSettings) : this(dbSettings.Value)
        {
        }

        public ZuneNetContext(ZuneNetContextSettings dbSettings)
        {
            MongoClient mongoClient = new(dbSettings.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(dbSettings.DatabaseName);

            _memberCollection = mongoDatabase.GetCollection<Member>(dbSettings.MemberCollectionName);
            _authCollection = mongoDatabase.GetCollection<TokenCidEntry>(dbSettings.AuthCollectionName);
        }

        public async Task<List<Member>> GetAsync(Expression<Func<Member, bool>> filter = null) =>
            await _memberCollection.Find(filter ?? (_ => true)).ToListAsync();

        public async Task<Member?> GetSingleAsync(Expression<Func<Member, bool>> filter = null) =>
            await _memberCollection.Find(filter ?? (_ => true)).FirstOrDefaultAsync();

        public async Task<Member?> GetAsync(Guid id) =>
            await _memberCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public Task<Member?> GetByIdOrZuneTag(string value)
        {
            if (Guid.TryParse(value, out var id))
                return GetAsync(id);
            return GetSingleAsync(m => m.ZuneTag == value);
        }

        public async Task CreateAsync(Member newMember) =>
            await _memberCollection.InsertOneAsync(newMember);

        public async Task UpdateAsync(Guid id, Member updatedMember) =>
            await _memberCollection.ReplaceOneAsync(x => x.Id == id, updatedMember);

        public async Task RemoveAsync(Guid id) =>
            await _memberCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<TokenCidEntry> GetCidByToken(string token)
        {
            string tokenHash = Hash(token);
            return await _authCollection.Find(e => e.TokenHash == tokenHash).FirstOrDefaultAsync();
        }

        public async Task<Member> GetMemberByToken(string token)
        {
            var entry = await GetCidByToken(token);
            if (entry == null)
                return null;

            return await GetSingleAsync(m => m.Cid == entry.Cid);
        }

        public async Task AddOrUpdateToken(string token, string cid)
        {
            string tokenHash = Hash(token);
            await _authCollection.DeleteManyAsync(e => e.TokenHash == token);
            await _authCollection.InsertOneAsync(new(tokenHash, cid));
        }

        private static string Hash(string str)
        {
            byte[] hash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(str));

            string[] hashStr = new string[hash.Length];
            for (int i = 0; i < hash.Length; i++)
                hashStr[i] = hash[i].ToString("X2");

            return string.Join(string.Empty, hashStr).ToUpperInvariant();
        }
    }
}

