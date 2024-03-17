using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechObjektBackend.Models;

namespace TechObjektBackend.Services
{
    public class VaccinationDataService
    {
        private readonly IMongoCollection<VaccinationData> _vaccinationDataCollection;

        public VaccinationDataService(IOptions<ProjectDatabaseSettings> projectDatabaseSettings)
        {
            var mongoClient = new MongoClient(projectDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(projectDatabaseSettings.Value.DatabaseName);
            _vaccinationDataCollection = mongoDatabase.GetCollection<VaccinationData>(projectDatabaseSettings.Value.VaccinationDataCollectionName);
        }

        public async Task<List<VaccinationData>> GetAsync() =>
            await _vaccinationDataCollection.Find(_ => true).ToListAsync();

        public async Task<VaccinationData> GetAsync(string id) =>
            await _vaccinationDataCollection.Find(data => data.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(VaccinationData data) =>
            await _vaccinationDataCollection.InsertOneAsync(data);

        public async Task UpdateAsync(string id, VaccinationData updatedData) =>
            await _vaccinationDataCollection.ReplaceOneAsync(data => data.Id == id, updatedData);

        public async Task RemoveAsync(string id) =>
            await _vaccinationDataCollection.DeleteOneAsync(data => data.Id == id);
    }
}
