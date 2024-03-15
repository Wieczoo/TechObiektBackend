using System;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TechObjektBackend.Models;

namespace TechObjektBackend.Services
{
	public class PrisonersServices
	{

        private readonly IMongoCollection<Prisoners> _PrisonersCollection;

        public PrisonersServices(IOptions<ProjectDatabaseSettings> projectDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                projectDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                projectDatabaseSettings.Value.DatabaseName);

            _PrisonersCollection = mongoDatabase.GetCollection<Prisoners>(
                projectDatabaseSettings.Value.PrisonersCollectionName);
        }

        public async Task<List<Prisoners>> GetAsync() =>
            await _PrisonersCollection.Find(_ => true).ToListAsync();

        public async Task<Prisoners?> GetAsync(string id) =>
            await _PrisonersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Prisoners newBook) =>
            await _PrisonersCollection.InsertOneAsync(newBook);

        public async Task UpdateAsync(string id, Prisoners updatedBook) =>
            await _PrisonersCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

        public async Task RemoveAsync(string id) =>
            await _PrisonersCollection.DeleteOneAsync(x => x.Id == id);
    }
}

