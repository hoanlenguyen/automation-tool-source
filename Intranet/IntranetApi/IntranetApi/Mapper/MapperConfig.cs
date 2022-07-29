using IntranetApi.Enum;
using IntranetApi.Models;
using Mapster;

namespace IntranetApi.Mapper
{
    public static class MapperConfig
    {
        public static void AddMapperConfigs()
        {
            TypeAdapterConfig<EmployeeExcelInput, Employee>.NewConfig()
                            .Map(dest => dest.Status, src => src.StatusStr.Equals(StatusValue.Active,StringComparison.OrdinalIgnoreCase))
                            .Map(dest => dest.IsDeleted, src => false)
                            ;

            TypeAdapterConfig<EmployeeExcelInput, EmployeeBulkInsert>.NewConfig()
                            .Map(dest => dest.BrandIds, src => string.Join(',', src.BrandIdList))
                            .Map(dest => dest.Status, src => src.StatusStr.Equals(StatusValue.Active, StringComparison.OrdinalIgnoreCase))
                            .Map(dest => dest.IsDeleted, src => false)
                            ;

            TypeAdapterConfig<EmployeeExcelInput, EmployeeImportError>.NewConfig()
                            .Ignore(p => p.Cells)
                            .Ignore(p => p.ErrorDetails)
                            .Map(dest => dest.Salary, src => src.SalaryStr)
                            .Map(dest => dest.BirthDate, src => src.BirthDateStr)
                            .Map(dest => dest.StartDate, src => src.StartDateStr)
                            ;

            TypeAdapterConfig<EmployeeExcelInput, User>.NewConfig()
                           .Map(dest => dest.Email, src => $"{src.EmployeeCode}@intranet.com")
                           .Map(dest => dest.FullName, src => src.Name)
                           .Map(dest => dest.UserName, src => src.BackendUser)
                           ;
        }
    }
}
