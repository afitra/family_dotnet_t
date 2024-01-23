using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using familyMart.Master;
using Microsoft.IdentityModel.Tokens;

namespace familyMart.Helpers;

public class Helper
{
    protected string _source;
    protected readonly BasicLogger _basicLogger;

    public Helper(BasicLogger basicLogger)
    {
        _source = GetType().Name;
        _basicLogger = basicLogger;
    }


    public static string StringToByte(string data)
    {
        if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(BasicConfiguration.GetVariableGlobal("PASS_SECRET")))
            throw new ArgumentException("Data dan kunci tidak boleh kosong.");

        try
        {
            // Menggunakan SHA-256 untuk menghasilkan kunci yang sesuai dengan panjang yang dibutuhkan oleh AES-256.
            using var sha256 = new SHA256CryptoServiceProvider();
            var keyBytes =
                sha256.ComputeHash(Encoding.UTF8.GetBytes(BasicConfiguration.GetVariableGlobal("PASS_SECRET")));

            using var aesAlg = Aes.Create();
            aesAlg.Key = keyBytes;

            // IV (Initialization Vector) digunakan untuk meningkatkan keamanan enkripsi.
            // IV harus unik dan tidak dirahasiakan.
            aesAlg.IV = new byte[aesAlg.BlockSize / 8];

            using var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(data);
            }

            return Convert.ToBase64String(aesAlg.IV.Concat(msEncrypt.ToArray()).ToArray());
        }
        catch (Exception ex)
        {
            throw new Exception($"Gagal mengenkripsi data. Error: {ex.Message}");
        }
    }


    public static bool StringToBoolean(string value)
    {
        if (bool.TryParse(value, out var result))
            return result;

        return false;
    }

    public static int StringToInteger(string value)
    {
        if (int.TryParse(value, out var result))
            return result;

        return 0;
    }

    public static string HashPassword(string password, string hashAlgorithm = "SHA256")
    {
        using (var hasher = HashAlgorithm.Create(hashAlgorithm))
        {
            if (hasher == null)
            {
                throw new ArgumentException($"Invalid hash algorithm: {hashAlgorithm}");
            }

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = Encoding.UTF8.GetBytes(BasicConfiguration.GetVariableGlobal("PASS_SECRET"));

            // Combine salt and password bytes using ||
            byte[] combinedBytes = new byte[passwordBytes.Length + saltBytes.Length];
            Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, combinedBytes, saltBytes.Length, passwordBytes.Length);

            byte[] hashBytes = hasher.ComputeHash(combinedBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }


    public static bool VerifyPassword(string password, string hashedPassword)
    {
        string newHashedPassword = HashPassword(password);
        return string.Equals(newHashedPassword, hashedPassword);
    }

    public static string GenerateToken(object payload)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var signingKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(BasicConfiguration.GetVariableGlobal(("JWT_SECRET"))));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
        };

        foreach (var property in payload.GetType().GetProperties())
        {
            tokenDescriptor.Subject.AddClaim(new Claim(property.Name, property.GetValue(payload)?.ToString()));
        }

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public static bool VerifyToken(string token, out object payload)
    {
        payload = null;

        var tokenHandler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(BasicConfiguration.GetVariableGlobal("JWT_SECRET"))),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };

        try
        {
            SecurityToken validatedToken;
            ClaimsPrincipal claimsPrincipal =
                tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

            payload = new ExpandoObject();
            foreach (var claim in claimsPrincipal.Claims)
            {
                ((IDictionary<string, object>)payload)[claim.Type] = claim.Value;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }


    public static string GenerateNIK(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        if (length <= 0)
        {
            throw new ArgumentException("Length should be greater than zero.");
        }

        Random random = new Random();
        StringBuilder nikBuilder = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            nikBuilder.Append(chars[random.Next(chars.Length)]);
        }

        return nikBuilder.ToString();
    }

    public static string GenerateRandomString(string prefix, int length)
    {
        const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        StringBuilder stringBuilder = new StringBuilder();

        // Tambahkan prefix
        stringBuilder.Append(prefix);

        // Tambahkan karakter acak
        Random random = new Random();
        for (int i = prefix.Length; i < length; i++)
        {
            int randomIndex = random.Next(0, allowedChars.Length);
            stringBuilder.Append(allowedChars[randomIndex]);
        }

        return stringBuilder.ToString();
    }
}