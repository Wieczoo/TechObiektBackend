using System;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TechObjektBackend.Models;
using MongoDB.Bson;
using FluentAssertions.Equivalency;


namespace TechObjektBackend.Services
{
    public class QueryBuilderServices
    {

        private readonly IMongoDatabase _mongoDatabase;
        //private readonly IMongoCollection<Prisoners> _PrisonersCollection;

        public QueryBuilderServices(IOptions<ProjectDatabaseSettings> projectDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                projectDatabaseSettings.Value.ConnectionString);

            _mongoDatabase = mongoClient.GetDatabase(
                projectDatabaseSettings.Value.DatabaseName);
        }

        public async Task<List<string>> GetCollectionsNames()
        {
            var collections = await _mongoDatabase.ListCollectionsAsync();
            var collectionNames = new List<string>();

            await collections.ForEachAsync(async document =>
            {
                collectionNames.Add(document["name"].AsString);
            });

            return collectionNames;
        }

        public async Task<List<ColumnsNamesWithType>> GetColumnsNames(string collectionName)
        {
            var collection = _mongoDatabase.GetCollection<BsonDocument>(collectionName);
            List<ColumnsNamesWithType> columns = new List<ColumnsNamesWithType>();
            // Pobierz pierwszy dokument z kolekcji, aby uzyskać listę pól
            var firstDocument = await collection.Find(new BsonDocument()).FirstOrDefaultAsync();

            if (firstDocument == null)
            {
                throw new Exception($"Collection '{collectionName}' is empty or does not exist.");
            }


           

            // Pobierz listę nazw pól z pierwszego dokumentu
            foreach (var element in firstDocument.Elements)
            {
                if (element.Name == "values" && element.Value.IsBsonArray)
                {
                    var nestedArray = element.Value.AsBsonArray;
                    if (nestedArray.Count > 0 && nestedArray[0].IsBsonDocument)
                    {
                        var nestedObject = nestedArray[0].AsBsonDocument;
                        foreach (var nestedElement in nestedObject.Elements)
                        {
                            columns.Add(new ColumnsNamesWithType ( $"values.{nestedElement.Name}", nestedElement.GetType().ToString()));
                        }
                    }
                }
                else
                {
                    columns.Add(new ColumnsNamesWithType (element.Name, element.Value.GetType().ToString()));
                }
            }

            return columns;
        }


        public async Task<List<BsonDocument>> GetData(string collectionName, DocumentQuery filterConditions)
        {
            var collection = _mongoDatabase.GetCollection<BsonDocument>(collectionName);
            var filter = CreateFilterMongo(filterConditions);

            var documents = await collection.Find(filter).ToListAsync();
            return documents;
        }

        public static BsonDocument CreateFilterMongo(DocumentQuery filterConditions)
        {
            var filters = new List<BsonDocument>();
            foreach (var conditionGroup in filterConditions.FilterConditions)
            {
                var conditionFilters = new List<BsonDocument>();
                foreach (var condition in conditionGroup)
                {
                    var filter = new BsonDocument();
                    dynamic valueOperator = null;
                    if (condition.Type == "number")
                    {
                        valueOperator = Convert.ToInt32(condition.Operator);
                    }
                    else if (condition.Type == "text")
                    {
                        valueOperator = condition.Operator;
                    }
                    switch (condition.FilterType.ToLower())
                    {
                        case "gt":
                            filter = new BsonDocument("$gt", valueOperator);
                            break;
                        case "lt":
                            filter = new BsonDocument("$lt", valueOperator);
                            break;
                        case "eq":
                            filter = new BsonDocument("$eq", valueOperator);
                            break;
                        case "ne":
                            filter = new BsonDocument("$ne", valueOperator);
                            break;
                        default:
                            throw new ArgumentException($"Nieobsługiwany typ filtra: {condition.FilterType}");
                    }

                    conditionFilters.Add(new BsonDocument(condition.ColumnName, filter));
                }
                filters.Add(new BsonDocument("$and", new BsonArray(conditionFilters)));
            }
            return new BsonDocument("$or", new BsonArray(filters));
        }
    }
}
