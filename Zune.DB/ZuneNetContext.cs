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

        public ZuneNetContext(IOptions<ZuneNetContextSettings> dbSettings) : this(dbSettings.Value)
        {
        }

        public ZuneNetContext(ZuneNetContextSettings dbSettings)
        {
            MongoClient mongoClient = new(dbSettings.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(dbSettings.DatabaseName);

            _memberCollection = mongoDatabase.GetCollection<Member>(dbSettings.MemberCollectionName);
        }

        public async Task<List<Member>> GetAsync(Expression<Func<Member, bool>> filter = null) =>
            await _memberCollection.Find(filter ?? (_ => true)).ToListAsync();

        public async Task<Member?> GetSingleAsync(Expression<Func<Member, bool>> filter = null) =>
            await _memberCollection.Find(filter ?? (_ => true)).FirstOrDefaultAsync();

        public async Task<Member?> GetAsync(MongoDB.Bson.ObjectId id) =>
            await _memberCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Member newMember) =>
            await _memberCollection.InsertOneAsync(newMember);

        public async Task UpdateAsync(MongoDB.Bson.ObjectId id, Member updatedMember) =>
            await _memberCollection.ReplaceOneAsync(x => x.Id == id, updatedMember);

        public async Task RemoveAsync(MongoDB.Bson.ObjectId id) =>
            await _memberCollection.DeleteOneAsync(x => x.Id == id);
    }
}

