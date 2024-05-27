using System;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TechObjektBackend.Models;
using Microsoft.ML;
using Microsoft.ML.Trainers;


namespace TechObjektBackend.Services
{
	public class NewBuildingsServices
    {

        private readonly IMongoCollection<NewBuildings> _NewBuildingsCollection;
        private readonly MLContext _mlContext;

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




        public async Task<List<YearData>> PredictFutureData(string id)
        {
            // Load data from MongoDB
            var existingData = await GetAsync(id);
            if (existingData == null)
            {
                // Obsłuż sytuację, gdy nie znaleziono danych o podanym identyfikatorze
                // Na przykład możesz zwrócić pustą listę lub odpowiedni komunikat błędu.
                return new List<YearData>();
            }
            // Convert data to format suitable for ML.NET
            var trainingData = existingData.Values.Select(x => new YearDataForPrediction { Year = float.Parse(x.Year), Val = x.Val }).ToList();

            // Train model using ML.NET
            var pipeline = _mlContext.Transforms.Concatenate("Features", nameof(YearDataForPrediction.Year))
                .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: nameof(YearDataForPrediction.Val), maximumNumberOfIterations: 100));

            var trainingDataView = _mlContext.Data.LoadFromEnumerable(trainingData);
            var model = pipeline.Fit(trainingDataView);

            // Predict future values
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<YearDataForPrediction, YearDataPrediction>(model);
            var futureData = Enumerable.Range(1, 5) // Predicting 5 future years
                .Select(year => new YearDataForPrediction { Year = existingData.Values.Max(x => float.Parse(x.Year)) + year })
                .Select(predictionEngine.Predict)
                .Select(prediction => new YearData { Year = prediction.Year.ToString(), Val = (int)prediction.Val })
                .ToList();

            return futureData;
        }
    }
}

