using System;
using MongoDB.Bson;
using TechObjektBackend.Models;

namespace TechObjektBackend.Middleware
{
	public class CreateFilter
	{
        public static BsonDocument CreateFilterMongo(List<FilterCondition> filterConditions)
        {
            var filters = new List<BsonDocument>();

            foreach (var condition in filterConditions)
            {
                BsonValue value;
                switch (condition.FilterType.ToLower())
                {
                    case "string":
                        value = condition.Operator;
                        break;
                    case "number":
                        value = double.Parse(condition.Operator);
                        break;
                    // Dodaj obsługę innych typów danych, jeśli jest taka potrzeba
                    default:
                        throw new ArgumentException($"Nieobsługiwany typ filtra: {condition.FilterType}");
                }

                var filter = new BsonDocument("$" + condition.Operator, value);
                filters.Add(new BsonDocument(condition.ColumnName, filter));
            }

            return new BsonDocument("$and", new BsonArray(filters));
        }
    }
}

