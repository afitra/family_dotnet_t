using familyMart.Request;

namespace familyMart.Interface;

public interface IEmployeeUsecase
{
    Task<object> RegisterEmployeeStoreProcedureAsync(RegisterEmployeeRequest request);
    Task<object> LoginAsync(LoginEmployeeRequest request);
    Task<object> AbsenAsync(string nik, AbsenEmployeeRequest request);
    Task<object> RekapAsync(string nik, RekapEmployeeRequest request);
    Task<object> UploadProfileAsync(string nik, UploadProfileRequest request);
}