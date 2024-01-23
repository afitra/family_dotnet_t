using familyMart.DTO;
using familyMart.Model;
using Microsoft.EntityFrameworkCore.Storage;

namespace familyMart.Interface;

public interface IAbsenRepository
{
    Task<IDbContextTransaction> BeginTransaction();
    void CommitTransaction();
    void RollbackTransaction();
    Task<Absen> GetAbsenEmployeeOnDate(int employeeId, DateOnly tanggal);
    Task<AbsenDTO> InsertAbsenAsync(Absen reqquest);
    Task<List<AbsenDTO>> GetAbsenEmployeeOnMonth(int employeeId, string tanggal, string tahun);
}