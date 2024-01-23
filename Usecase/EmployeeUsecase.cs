using System.Runtime.InteropServices.ComTypes;
using AutoMapper;
using familyMart.DTO;
using familyMart.Helpers;
using familyMart.Interface;
using familyMart.Master;
using familyMart.Model;
using familyMart.Request;

namespace familyMart.Usecase;

public class EmployeeUsecase : IEmployeeUsecase
{
    protected string _source;
    protected readonly IMapper _mapper;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAbsenRepository _absenRepository;


    public EmployeeUsecase(IMapper mapper, IEmployeeRepository employeeRepository, IAbsenRepository absenRepository)
    {
        _source = GetType().Name;
        _mapper = mapper;
        _employeeRepository = employeeRepository;
        _absenRepository = absenRepository;
    }

    public async Task<object> RegisterEmployeeStoreProcedureAsync(RegisterEmployeeRequest request)
    {
        var existingEmployee = await _employeeRepository.GetEmployeeByEmailAsync(request.Email);

        if (existingEmployee != null) throw new Exception(BasicMessage.DataAlreadyExistErrorMessage);

        var payload = new Employee
        {
            Nama = request.Nama,
            Nik = Helper.GenerateNIK(8),
            Email = request.Email,
            Password = request.Password
        };

        await _employeeRepository.RegisterEmployeeStoreProsedureAsync(payload,
            BasicConfiguration.GetVariableGlobal("PASS_SECRET"));

        var claim = new { payload.Nik, payload.Email };

        return new { Token = Helper.GenerateToken(claim) };
    }

    public async Task<object> RegisterEmployeeAsync(RegisterEmployeeRequest request)
    {
        var existingEmployee = await _employeeRepository.GetEmployeeByEmailAsync(request.Email);

        if (existingEmployee != null) throw new Exception(BasicMessage.DataAlreadyExistErrorMessage);

        var Payload = new Employee
        {
            Nama = request.Nama,
            Nik = Helper.GenerateNIK(8),
            Email = request.Email,
            Password = Helper.HashPassword(request.Password)
        };
        var employee = await _employeeRepository.InsertEmployeeAsync(Payload);

        var claim = new { employee.Nik, employee.Email };
        return new { Token = Helper.GenerateToken(claim) };
    }


    public async Task<object> LoginAsync(LoginEmployeeRequest request)
    {
        var employee = await _employeeRepository.GetEmployeeByEmailAsync(request.Email);

        if (employee == null) throw new Exception(BasicMessage.LoginErrorMessage);

        if (!Helper.VerifyPassword(request.Password, employee.Password))
            throw new Exception(BasicMessage.LoginErrorMessage);

        var payload = new { employee.Email, employee.Nik };

        return new { Token = Helper.GenerateToken(payload) };
    }

    public async Task<object> AbsenAsync(string nik, AbsenEmployeeRequest request)
    {
        var employee = await _employeeRepository.GetEmployeeByNikAsync(nik);
        if (employee == null) throw new Exception(BasicMessage.AuthErrorMessage);

        var geotaggingArray = request.Geotagging?.Split(',');
        if (geotaggingArray == null || geotaggingArray.Length != 2)
        {
            throw new Exception(BasicMessage.GeneralErrorMessage);
        }

        var tgl = DateOnly.Parse(request.Tanggal);
        var absenData = await _absenRepository.GetAbsenEmployeeOnDate(employee.Id, tgl);

        if (absenData != null) throw new Exception(BasicMessage.AbsenAlreadyExistErrorMessage);

        var payload = new Absen
        {
            Employee_id = employee.Id,
            Tanggal = tgl,
            Geotagging = request.Geotagging
        };

        var absen = await _absenRepository.InsertAbsenAsync(payload);

        return new { absen };
    }

    public async Task<object> RekapAsync(string nik, RekapEmployeeRequest request)
    {
        var employee = await _employeeRepository.GetEmployeeByNikAsync(nik);
        if (employee == null) throw new Exception(BasicMessage.AuthErrorMessage);

        var rekap = await _absenRepository.GetAbsenEmployeeOnMonth(employee.Id, request.Bulan, request.Tahun);


        return new { rekap };
    }
}