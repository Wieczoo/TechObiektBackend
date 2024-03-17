using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;



    public class Education
    {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]

    public string Id { get; set; }

    public string nazwa_zmiennej { get; set; }

    public string kraj { get; set; }

    public string wojewodztwo { get; set; }

    public string typ_szkoly { get; set; }

    public string plec_absolwenta { get; set; }

    public string rodzaj_wskaznika { get; set; }

    public string typ_informacji_z_jednostka_miary { get; set; }

    public string rok_szkolny { get; set; }

    public double wartosc { get; set; }

    public int flaga { get; set; }

}

