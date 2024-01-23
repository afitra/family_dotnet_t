using AutoMapper;
using familyMart.Connection;
using familyMart.DTO;
using familyMart.Interface;
using familyMart.Master;
using familyMart.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using NpgsqlTypes;
using Microsoft.EntityFrameworkCore;


namespace familyMart.Repository;

public class EmployeeRepository : IEmployeeRepository
{
    protected string _source;
    private readonly BasicDatabase _dbContext;
    private readonly IMapper _mapper;

    public EmployeeRepository(BasicDatabase dbContext, IMapper mapper)
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

    public async Task<EmployeeDTO> InsertEmployeeAsync(Employee employee)
    {
        _dbContext.Employees.Add(employee);
        await _dbContext.SaveChangesAsync();
        return _mapper.Map<EmployeeDTO>(employee);
    }


    public async Task<Employee> GetEmployeeByEmailAsync(string email)
    {
        return await _dbContext.Employees.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Employee> GetEmployeeByNikAsync(string nik)
    {
        return await _dbContext.Employees.FirstOrDefaultAsync(u => u.Nik == nik);
    }


    public async Task RegisterEmployeeStoreProsedureAsync(Employee payload, string salt)
    {
        string query = "CALL sp_RegisterEmployee(@p_Nama, @p_Nik, @p_Email, @p_Password, @p_Salt);";

        var parameters = new[]
        {
            new NpgsqlParameter("@p_Nama", NpgsqlDbType.Varchar) { Value = payload.Nama },
            new NpgsqlParameter("@p_Nik", NpgsqlDbType.Varchar) { Value = payload.Nik },
            new NpgsqlParameter("@p_Email", NpgsqlDbType.Varchar) { Value = payload.Email },
            new NpgsqlParameter("@p_Password", NpgsqlDbType.Varchar) { Value = payload.Password },
            new NpgsqlParameter("@p_Salt", NpgsqlDbType.Varchar) { Value = salt }
        };

        await _dbContext.Database.ExecuteSqlRawAsync(query, parameters);
    }

    public async Task UpdateProfileUrlByNikAsync(string nik, string profileUrl)
    {
        await _dbContext.Database.ExecuteSqlRawAsync(
            $"UPDATE \"Employees\" SET \"Profile_url\" = {{0}} WHERE \"Nik\" = {{1}}",
            profileUrl, nik);
    }
}