using IntranetApi.Enum;
using IntranetApi.Models;
using Mapster;

namespace IntranetApi.Mapper
{
    public static class MapperConfig
    {
        public static void AddMapperConfigs()
        {
            TypeAdapterConfig<RankCreateOrEdit, Rank>.NewConfig()
                            .Ignore(p => p.CreationTime)
                            .Map(dest => dest.IsDeleted, src => false)
                            ;

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
                           .Map(dest => dest.UserName, src => src.BackendUser)
                           ;

            TypeAdapterConfig<EmployeeCreateOrEdit, Employee>.NewConfig()
                           .Map(dest => dest.BrandEmployees, src => src.BrandIds.Select(p=>new BrandEmployee { BrandId=p, EmployeeId= src.Id}))
                           ;

            TypeAdapterConfig<Employee, EmployeeCreateOrEdit>.NewConfig()
                           .Map(dest => dest.BrandIds, src => src.BrandEmployees.Select(p => p.BrandId).ToList())
                           ;

            TypeAdapterConfig<StaffRecord, StaffRecordCreateOrEdit>.NewConfig()
                           .Map(dest => dest.StaffRecordDocuments, src => src.StaffRecordDocuments.Select(p=>p.FileUrl).ToList())
                           ;

            TypeAdapterConfig<StaffRecordCreateOrEdit, StaffRecord>.NewConfig()
                           .Map(dest => dest.StaffRecordDocuments, src => src.StaffRecordDocuments.Select(p => new StaffRecordDocument { FileUrl = p }).ToList())
                           ;

            TypeAdapterConfig<StaffRecord, StaffRecordList>.NewConfig()
                           ;

            
        }
    }
}
