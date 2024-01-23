using System.Drawing;
using NetTopologySuite.Geometries;

namespace familyMart.Model;

public class Absen
{
    public int Id { get; set; }
    public int Employee_id { get; set; }
    public DateOnly Tanggal { get; set; }
    public String Geotagging { get; set; }
}