using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using familyMart.Helpers;
using familyMart.Interface;
using familyMart.Master;
using familyMart.Middleware;
using familyMart.Request;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace familyMart.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : BasicController
{
    private readonly IEmployeeUsecase _employeeUsecase;


    public EmployeeController(BasicLogger basicLogger, BasicConfiguration basicConfiguration, IMapper mapper,
        IEmployeeUsecase employeeUsecase)
        : base(basicLogger, basicConfiguration, mapper)
    {
        _source = GetType().Name;
        _employeeUsecase = employeeUsecase;
    }


    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterEmployeeRequest request)
    {
        try
        {
            await _employeeUsecase.RegisterEmployeeStoreProcedureAsync(request);
            return SetResponse(StatusCodes.Status201Created, BasicCode.GeneralCode, true,
                BasicMessage.DataRegisteredMessage);
        }
        catch (Exception e)
        {
            return SetErrorResponse(e);
        }
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginEmployeeRequest request)
    {
        try
        {
            var data = await _employeeUsecase.LoginAsync(request);
            return SetResponse(StatusCodes.Status200OK, BasicCode.GeneralCode, true, BasicMessage.GeneralMessage,
                data);
        }
        catch (Exception e)
        {
            return SetErrorResponse(e);
        }
    }


    // -6.176209,106.839732
    [HttpPost("absen")]
    [MiddlewareFilter(typeof(JwtAuthenticationMiddleware))]
    public async Task<IActionResult> Absen([FromBody] AbsenEmployeeRequest request)
    {
        try
        {
            var nikClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Nik")?.Value;
            var data = await _employeeUsecase.AbsenAsync(nikClaim, request);
            return SetResponse(StatusCodes.Status200OK, BasicCode.GeneralCode, true, BasicMessage.DataAddedMessage,
                data);
        }
        catch (Exception e)
        {
            return SetErrorResponse(e);
        }
    }


    [HttpPost("absen/rekap")]
    [MiddlewareFilter(typeof(JwtAuthenticationMiddleware))]
    public async Task<IActionResult> Absen([FromBody] RekapEmployeeRequest request)
    {
        try
        {
            // foreach (var claim in User.Claims) Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");

            var nikClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Nik")?.Value;
            var data = await _employeeUsecase.RekapAsync(nikClaim, request);
            return SetResponse(StatusCodes.Status200OK, BasicCode.GeneralCode, true, BasicMessage.DataAddedMessage,
                data);
        }
        catch (Exception e)
        {
            return SetErrorResponse(e);
        }
    }


    [HttpPost("upload/profile")]
    [MiddlewareFilter(typeof(JwtAuthenticationMiddleware))]
    public async Task<IActionResult> UploadPhoto([FromForm] UploadProfileRequest request)
    {
        var nikClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Nik")?.Value;
        var data = await _employeeUsecase.UploadProfileAsync(nikClaim, request);
        return SetResponse(StatusCodes.Status200OK, BasicCode.GeneralCode, true, BasicMessage.DataAddedMessage,
            data);
    }
}