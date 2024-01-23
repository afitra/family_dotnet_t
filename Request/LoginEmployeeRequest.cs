using System.ComponentModel.DataAnnotations;

namespace familyMart.Request;

public class LoginEmployeeRequest
{
    [Required(ErrorMessage = "Email tidak boleh kosong")]
    [EmailAddress(ErrorMessage = "Email tidak valid")]
    //
    public string Email { get; set; }

    [Required(ErrorMessage = "Password tidak boleh kosong")]
    [MinLength(6, ErrorMessage = "Password harus memiliki setidaknya 6 karakter")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$", ErrorMessage = "Password tidak valid")]
    //
    public string Password { get; set; }
}

public class SwaggerExampleAttribute : Attribute
{
    public SwaggerExampleAttribute(string user1, string abcd1234)
    {
        throw new NotImplementedException();
    }
}