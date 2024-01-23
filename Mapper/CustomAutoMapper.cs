using AutoMapper;
using familyMart.DTO;
using familyMart.Model;

namespace familyMart.Mapper;

public class CustomAutoMapper : Profile
{
    public CustomAutoMapper()
    {
        CreateMap<Employee, EmployeeDTO>();
        CreateMap<Absen, AbsenDTO>();
    }
}