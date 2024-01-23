using familyMart.DTO;
using familyMart.Model;
using Microsoft.EntityFrameworkCore.Storage;

namespace familyMart.Interface;

public interface IEmployeeRepository
{
    Task<IDbContextTransaction> BeginTransaction();
    void CommitTransaction();
    void RollbackTransaction();
    Task<EmployeeDTO> InsertEmployeeAsync(Employee user);
    Task<Employee> GetEmployeeByEmailAsync(string email);
    Task<Employee> GetEmployeeByNikAsync(string nik);
    Task RegisterEmployeeStoreProsedureAsync(Employee payload, string salt);
    Task UpdateProfileUrlByNikAsync(string nik, string profileUrl);
}