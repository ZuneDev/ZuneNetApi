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
        private readonly IMongoCollection<TokenEntry> _authCollection;
        private readonly IMongoCollection<ImageEntry> _imageCollection;
        private readonly IMongoCollection<WMISAlbumIdEntry> _albumLookupCollection;
        private readonly IMongoCollection<WMISAlbumTrackEntry> _trackLookupCollection;

        public ZuneNetContext(IOptions<ZuneNetContextSettings> dbSettings) : this(dbSettings.Value)
        {
        }

        public ZuneNetContext(ZuneNetContextSettings dbSettings)
        {
            MongoClient mongoClient = new(dbSettings.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(dbSettings.DatabaseName);

            _memberCollection = mongoDatabase.GetCollection<Member>(dbSettings.MemberCollectionName);
            _authCollection = mongoDatabase.GetCollection<TokenEntry>(dbSettings.AuthCollectionName);
            _imageCollection = mongoDatabase.GetCollection<ImageEntry>(dbSettings.ImageCollectionName);
            _albumLookupCollection = mongoDatabase.GetCollection<WMISAlbumIdEntry>(dbSettings.AlbumLookupCollectionName);
            _trackLookupCollection = mongoDatabase.GetCollection<WMISAlbumTrackEntry>(dbSettings.TrackLookupCollectionName);
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

        public async Task UpdateAsync(Member updatedMember) =>
            await _memberCollection.ReplaceOneAsync(x => x.Id == updatedMember.Id, updatedMember);

        public async Task UpdateAsync(Guid id, Member updatedMember) =>
            await _memberCollection.ReplaceOneAsync(x => x.Id == id, updatedMember);

        public async Task RemoveAsync(Guid id) =>
            await _memberCollection.DeleteOneAsync(x => x.Id == id);

        public Task ClearMembersAsync() => _memberCollection.DeleteManyAsync(_ => true);

        public async Task<TokenEntry> GetCidByToken(string token)
        {
            string tokenHash = Helpers.Hash(token);
            Console.WriteLine($"hashedToken = {tokenHash}");
            return await _authCollection.Find(e => e.TokenHash == tokenHash).FirstOrDefaultAsync();
        }

        public async Task<Member> GetMemberByName(string UserName)
        {
            return await GetSingleAsync(m => m.UserName == UserName);
        }

        public async Task<Member> GetMemberBySid(string sid)
        {
            return await GetSingleAsync(user => user.SID == sid);
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

        public async Task<Guid?> GetAlbumIdRecordAsync(Int64 id)
        {
            try
            {
                var existing = await _albumLookupCollection.FindAsync(x => x.AlbumId == id);
                var record = await existing.SingleAsync();
                return record.AlbumGuid;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Int64?> GetAlbumIdRecordAsync(Guid id)
        {
            try
            {
                var existing = await _albumLookupCollection.FindAsync(x => x.AlbumGuid == id);
                var record = await existing.SingleAsync();
                return record.AlbumId;
            }
            catch
            {
                return null;
            }
        }

        // It's ugly, but it maps a MBID to an Int64 for WMIS's crazy lookup
        public async Task<Int64> CreateOrGetAlbumIdInt64Async(Guid guid)
        {
            var existing = await GetAlbumIdRecordAsync(guid);
            if (existing.HasValue)
            {
                return existing.Value;
            }
            while (true)
            {
                var id = new Random().NextInt64();
                var found = await GetAlbumIdRecordAsync(id);
                if (!found.HasValue)
                {
                    await _albumLookupCollection.InsertOneAsync(new WMISAlbumIdEntry(id, guid));
                    return id;
                }
            }
        }

        public async Task<Guid?> GetTrackMbidFromTrackIdAndDurationAsync(int trackNumber, int trackDuration)
        {
            try
            {
                var record = await _trackLookupCollection.FindAsync(x => x.TrackId == trackNumber && x.TrackDuration == trackDuration);
                var first = await record.FirstOrDefaultAsync();
                return first.TrackMbid;
            }
            catch { return null; }
        }

        public async Task<Guid?> GetAlbumMbidFromTrackIdAndDurationAsync(int trackNumber, int trackDuration)
        {
            try
            {
                var record = await _trackLookupCollection.FindAsync(x => x.TrackId == trackNumber && x.TrackDuration == trackDuration);
                var first = await record.FirstOrDefaultAsync();
                return first.AlbumMbid;
            }
            catch { return null; }
        }

        public async Task CreateTrackReverseLookupRecordAsync(Guid albumMbid, Guid trackMbid, int trackNumber, int trackDuration)
        {
            if (await GetAlbumMbidFromTrackIdAndDurationAsync(trackNumber, trackDuration) == null)
            {
                await _trackLookupCollection.InsertOneAsync(
                new WMISAlbumTrackEntry(trackNumber, trackDuration, albumMbid, trackMbid));
            }
        }
    }
}

