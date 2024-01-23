using AutoMapper;
using familyMart.Connection;
using familyMart.DTO;
using familyMart.Interface;
using familyMart.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using NpgsqlTypes;

namespace familyMart.Repository;

public class AbsenRepository : IAbsenRepository
{
    protected string _source;
    private readonly BasicDatabase _dbContext;
    private readonly IMapper _mapper;

    public AbsenRepository(BasicDatabase dbContext, IMapper mapper)
    {
        _source = GetType().Name;
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _mapper = mapper;
    }

    public async Task<IDbContextTransaction> BeginTransaction()
    {
        return await _dbContext.Database.BeginTransactionAsync();
    }

    public void CommitTransaction()
    {
        _dbContext.Database.CommitTransaction();
    }

    public void RollbackTransaction()
    {
        _dbContext.Database.RollbackTransaction();
    }

    public async Task<Absen> GetAbsenEmployeeOnDate(int employeeId, DateOnly tanggal)
    {
        return await _dbContext.Absens
            .Where(a => a.Employee_id == employeeId && a.Tanggal == tanggal)
            .FirstOrDefaultAsync();
    }

    public async Task<AbsenDTO> InsertAbsenAsync(Absen absen)
    {
        _dbContext.Absens.Add(absen);
        await _dbContext.SaveChangesAsync();
        return _mapper.Map<AbsenDTO>(absen);
    }

    public async Task<List<AbsenDTO>> GetAbsenEmployeeOnMonth(int employeeId, string tanggal, string tahun)
    {
        string query =
            "SELECT * FROM public.getabsenbyemployeeandmonthfunction(@EmployeeId, @TargetMonth, @TargetYear);";

        var parameters = new[]
        {
            new NpgsqlParameter("@EmployeeId", NpgsqlDbType.Integer) { Value = employeeId },
            new NpgsqlParameter("@TargetMonth", NpgsqlDbType.Integer) { Value = Convert.ToInt32(tanggal) },
            new NpgsqlParameter("@TargetYear", NpgsqlDbType.Integer) { Value = Convert.ToInt32(tahun) }
        };

        var absenList = await _dbContext.Absens
            .FromSqlRaw(query, parameters)
            .ToListAsync();

        return _mapper.Map<List<AbsenDTO>>(absenList);
    }
}