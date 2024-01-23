using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace familyMart.Request;

public class RekapEmployeeRequest
{
    [Required(ErrorMessage = "Nama tidak boleh kosong")]
    [SwaggerSchema(Description = "format 01")]
    public string Bulan { get; set; }

    [Required(ErrorMessage = "Nama tidak boleh kosong")]
    [SwaggerSchema(Description = "format 2024")]
    public string Tahun { get; set; }
}