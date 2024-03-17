using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class VaccinationData
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string nazwa_zmiennej { get; set; }

    public string kraj { get; set; }

    public string rodzaj_choroby { get; set; }

    public string czas_typ_szczepienia { get; set; }

    public string typ_informacji_z_jednostka_miary { get; set; }

    public int rok { get; set; }

    public double wartosc { get; set; }

    public double zmienna { get; set; }
}
