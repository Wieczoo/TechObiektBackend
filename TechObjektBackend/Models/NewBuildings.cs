using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

public class NewBuildings
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("id")]
    public string IdRegion { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }


    [BsonElement("values")]
    public List<YearData> Values { get; set; }
}

public class YearData
{
    [BsonElement("year")]
    public string Year { get; set; }

    [BsonElement("val")]
    public int Val { get; set; }

    [BsonElement("attrId")]
    public int AttrId { get; set; }
}