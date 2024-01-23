using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

public class AbsenEmployeeRequest
{
    [Required(ErrorMessage = "Tanggal tidak boleh kosong")]
    [SwaggerSchema(Description = "format 2022-01-01")]
    public string Tanggal { get; set; }

    [Required(ErrorMessage = "Geotagging tidak boleh kosong")]
    public string Geotagging { get; set; }
}