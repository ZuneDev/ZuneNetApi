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
        private readonly IMongoCollection<TokenEntry> _authCollection;
        private readonly IMongoCollection<ImageEntry> _imageCollection;

        public ZuneNetContext(IOptions<ZuneNetContextSettings> dbSettings) : this(dbSettings.Value)
        {
        }

        public ZuneNetContext(ZuneNetContextSettings dbSettings)
        {
            if (dbSettings?.ConnectionString is null)
                throw new ZuneNetConfigurationException("MongoDB connection string was not provided.");

            if (dbSettings.DatabaseName is null)
                throw new ZuneNetConfigurationException("MongoDB database name was not provided.");
                
            var mongoClient = new MongoClient(dbSettings.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dbSettings.DatabaseName);

            _memberCollection = mongoDatabase.GetCollection<Member>(dbSettings.MemberCollectionName);
            _authCollection = mongoDatabase.GetCollection<TokenEntry>(dbSettings.AuthCollectionName);
            _imageCollection = mongoDatabase.GetCollection<ImageEntry>(dbSettings.ImageCollectionName);
        }

        public async Task<List<Member>> GetAsync(Expression<Func<Member, bool>> filter = null) =>
            await _memberCollection.Find(filter ?? (_ => true)).ToListAsync();

        public async Task<Member?> GetSingleAsync(Expression<Func<Member, bool>> filter = null) =>
            await _memberCollection.Find(filter ?? (_ => true)).FirstOrDefaultAsync();

        public async Task<Member?> GetAsync(Guid id) =>
            await _memberCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public Task<Member?> GetByIdOrZuneTag(string value)
        {
            return Guid.TryParse(value, out var id)
                ? GetAsync(id)
                : GetSingleAsync(m => m.ZuneTag == value);
        }

        public async Task CreateAsync(Member newMember) =>
            await _memberCollection.InsertOneAsync(newMember);

        public async Task UpdateAsync(Guid id, Member updatedMember) =>
            await _memberCollection.ReplaceOneAsync(x => x.Id == id, updatedMember);

        public async Task RemoveAsync(Guid id) =>
            await _memberCollection.DeleteOneAsync(x => x.Id == id);

        public Task ClearMembersAsync() => _memberCollection.DeleteManyAsync(_ => true);

        public async Task<TokenEntry> GetCidByToken(string token)
        {
            string tokenHash = Helpers.Hash(token);
            return await _authCollection.Find(e => e.TokenHash == tokenHash).FirstOrDefaultAsync();
        }

        public async Task<Member> GetMemberByToken(string token)
        {
            var entry = await GetCidByToken(token);
            if (entry == null)
                return null;

            return await GetSingleAsync(m => m.UserName == entry.UserName);
        }

        public async Task AddToken(string token, string userName)
        {
            string tokenHash = Helpers.Hash(token);
            await _authCollection.DeleteManyAsync(e => e.TokenHash == token);
            await _authCollection.InsertOneAsync(new(tokenHash, userName));
        }

        public Task ClearTokensAsync() => _authCollection.DeleteManyAsync(_ => true);

        public Task<ImageEntry> GetImageEntryAsync(Guid id) =>
            _imageCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<ImageEntry> AddImageAsync(string url)
        {
            ImageEntry entry = new(Helpers.GenerateGuid(url), url);

            if (await GetImageEntryAsync(entry.Id) == null)
            {
                await _imageCollection.InsertOneAsync(entry);
            }
            else
            {
                var update = Builders<ImageEntry>.Update.Set(nameof(ImageEntry.Url), url);
                await _imageCollection.UpdateOneAsync(e => e.Id == entry.Id, update);
            }

            return entry;
        }

        public Task ClearImagesAsync() => _imageCollection.DeleteManyAsync(_ => true);
    }
}

