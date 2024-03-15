using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Prisoners
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string VariableName { get; set; }

    
    public string Country { get; set; }

    
    public string PrisionerType { get; set; }

    
    public string CategoriesInmates { get; set; }

    
    public string Sex { get; set; }

    
    public string InformationTypeUnitofMeasure { get; set; }

    
    public int Year { get; set; }

    
    public int Value { get; set; }
}
