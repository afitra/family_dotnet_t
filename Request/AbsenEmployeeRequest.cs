using System.Drawing;

namespace familyMart.Request;

public class AbsenEmployeeRequest
{
    public int Id { get; set; }
    public string Tanggal { get; set; }
    public string Geotagging { get; set; }
}

public class GeotaggingModel
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}