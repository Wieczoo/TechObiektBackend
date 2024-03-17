using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechObjektBackend.Models;

namespace TechObjektBackend.Services
{
    public class EducationDataService
    {
        private readonly IMongoCollection<Education> _educationCollection;

        public EducationDataService(IOptions<ProjectDatabaseSettings> projectDatabaseSettings)
        {
            var mongoClient = new MongoClient(projectDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(projectDatabaseSettings.Value.DatabaseName);
            _educationCollection = mongoDatabase.GetCollection<Education>(projectDatabaseSettings.Value.EducationCollectionName);
        }

        public async Task<List<Education>> GetAsync() =>
            await _educationCollection.Find(_ => true).ToListAsync();

        public async Task<Education> GetAsync(string id) =>
            await _educationCollection.Find(data => data.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Education data) =>
            await _educationCollection.InsertOneAsync(data);

        public async Task UpdateAsync(string id, Education updatedData) =>
            await _educationCollection.ReplaceOneAsync(data => data.Id == id, updatedData);

        public async Task RemoveAsync(string id) =>
            await _educationCollection.DeleteOneAsync(data => data.Id == id);
    }
}
