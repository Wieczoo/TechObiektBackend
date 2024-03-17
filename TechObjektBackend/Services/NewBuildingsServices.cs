using System;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TechObjektBackend.Models;

namespace TechObjektBackend.Services
{
	public class NewBuildingsServices
    {

        private readonly IMongoCollection<NewBuildings> _NewBuildingsCollection;

        public NewBuildingsServices(IOptions<ProjectDatabaseSettings> projectDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                projectDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                projectDatabaseSettings.Value.DatabaseName);

            _NewBuildingsCollection = mongoDatabase.GetCollection<NewBuildings>(
                projectDatabaseSettings.Value.NewBuildingsCollectionName);
        }

        public async Task<List<NewBuildings>> GetAsync() =>
            await _NewBuildingsCollection.Find(_ => true).ToListAsync();

        public async Task<NewBuildings?> GetAsync(string id) =>
            await _NewBuildingsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(NewBuildings newBuilding) =>
            await _NewBuildingsCollection.InsertOneAsync(newBuilding);

        public async Task UpdateAsync(string id, NewBuildings updatedBuilding) =>
            await _NewBuildingsCollection.ReplaceOneAsync(x => x.Id == id, updatedBuilding);

        public async Task RemoveAsync(string id) =>
            await _NewBuildingsCollection.DeleteOneAsync(x => x.Id == id);
    }
}

