using System.Text.Json;
using System.Text.Json.Serialization;
namespace Library;

/// <summary>
/// Класс объекта MyType (в моём случае Station).
/// </summary>
[Serializable]
public class Station
{
    [JsonPropertyName("ID")]
    public int ID { get; set; }

    [JsonPropertyName("NameOfStation")]
    public string NameOfStation { get; set; }

    [JsonPropertyName("Line")]
    public string Line { get; set; }

    [JsonPropertyName("Longitude_WGS84")]
    public double Longitude { get; set; }

    [JsonPropertyName("Latitude_WGS84")]
    public double Latitude { get; set; }

    [JsonPropertyName("AdmArea")]
    public string AdmArea { get; set; }

    [JsonPropertyName("District")]
    public string District { get; set; }

    [JsonPropertyName("Year")]
    public int Year { get; set; }

    [JsonPropertyName("Month")]
    public string Month { get; set; }

    [JsonPropertyName("global_id")]
    public long GlobalId { get; set; }

    [JsonPropertyName("geodata_center")]
    public string GeodataCenter { get; set; }

    [JsonPropertyName("geoarea")]
    public string Geoarea { get; set; }

    public Station(int iD = 0, string nameOfStation = "", string line = "",
        double longitude = 0, double latitude = 0, string admArea = "",
        string district = "", int year = 0, string month = "",
        long globalId = 0, string geodataCenter = "", string geoarea = "")
    {
        this.ID = iD;
        this.NameOfStation = nameOfStation;
        this.Line = line;
        this.Longitude = longitude;
        this.Latitude = latitude;
        this.AdmArea = admArea;
        this.District = district;
        this.Year = year;
        this.Month = month;
        this.GlobalId = globalId;
        this.GeodataCenter = geodataCenter;
        this.Geoarea = geoarea;
    }
}