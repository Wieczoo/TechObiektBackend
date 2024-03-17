using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechObjektBackend.Models;

namespace TechObjektBackend.Services
{
    public class HeightDataService
    {
        private readonly IMongoCollection<Height> _heightCollection;

        public HeightDataService(IOptions<ProjectDatabaseSettings> projectDatabaseSettings)
        {
            var mongoClient = new MongoClient(projectDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(projectDatabaseSettings.Value.DatabaseName);
            _heightCollection = mongoDatabase.GetCollection<Height>(projectDatabaseSettings.Value.HeightCollectionName);
        }


        public async Task<List<Height>> GetAsync() =>
            await _heightCollection.Find(_ => true).ToListAsync();

        public async Task<Height> GetAsync(string id) =>
            await _heightCollection.Find(data => data.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Height data) =>
            await _heightCollection.InsertOneAsync(data);

        public async Task UpdateAsync(string id, Height updatedData) =>
            await _heightCollection.ReplaceOneAsync(data => data.Id == id, updatedData);

        public async Task RemoveAsync(string id) =>
            await _heightCollection.DeleteOneAsync(data => data.Id == id);
    }
}
