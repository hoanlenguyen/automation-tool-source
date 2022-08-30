using IntranetApi.Enum;
using IntranetApi.Models;
using Mapster;

namespace IntranetApi.Mapper
{
    public static class MapperConfig
    {
        public static void AddMapperConfigs()
        {
             
            TypeAdapterConfig<EmployeeExcelInput, EmployeeBulkInsert>.NewConfig()
                            .Map(dest => dest.Status, src => src.StatusStr.Equals(StatusValue.Active, StringComparison.OrdinalIgnoreCase))
                            .Map(dest => dest.IsDeleted, src => false)
                            ;

            TypeAdapterConfig<EmployeeBulkInsert, User>.NewConfig()
                            .Map(dest => dest.UserName, src => src.EmployeeCode)
                            .Map(dest => dest.Email, src => $"{src.EmployeeCode}@intranet.com")
                            .Map(dest => dest.BrandEmployees, src => src.BrandIds.Select(p=> new BrandEmployee { BrandId = p }).ToList())
                            ;

            TypeAdapterConfig<EmployeeExcelInput, EmployeeImportError>.NewConfig()
                            .Ignore(p => p.Cells)
                            .Ignore(p => p.ErrorDetails)
                            .Map(dest => dest.Salary, src => src.SalaryStr)
                            .Map(dest => dest.BirthDate, src => src.BirthDateStr)
                            .Map(dest => dest.StartDate, src => src.StartDateStr)
                            .Map(dest => dest.IntranetRole, src => src.Role)                             
                            ;

            TypeAdapterConfig<User, EmployeeExcelInput>.NewConfig()
                            .Map(dest => dest.BrandIds, src => src.BrandEmployees.Select(p=>p.BrandId).ToList())
                            .Map(dest => dest.Brand, src => string.Join(',', src.BrandEmployees.Select(q=>q.Brand.Name)))
                            ;

            TypeAdapterConfig<EmployeeExcelInput, User>.NewConfig()
                           .Map(dest => dest.Email, src => $"{src.EmployeeCode}@intranet.com")
                           .Map(dest => dest.UserName, src => src.BackendUser)
                           ;

            TypeAdapterConfig<EmployeeCreateOrEdit, User>.NewConfig()
                           .Map(dest => dest.UserName, src=>src.EmployeeCode)
                           .Map(dest => dest.UserRoles, src => new List<UserRole> { new UserRole {RoleId= src.RoleId } })
                           .Map(dest => dest.BrandEmployees, src => src.BrandIds.Select(p=>new BrandEmployee { BrandId=p, EmployeeId= src.Id}))
                           ;

            TypeAdapterConfig<User, EmployeeCreateOrEdit>.NewConfig()
                           .Map(dest => dest.BrandIds, src => src.BrandEmployees.Select(p => p.BrandId).ToList())
                           ;

            TypeAdapterConfig<StaffRecord, StaffRecordCreateOrEdit>.NewConfig()
                           .Map(dest => dest.RecordType, src => (int)src.RecordType)
                           .Map(dest => dest.RecordDetailType, src => (int)src.RecordDetailType)
                           .Map(dest => dest.StaffRecordDocuments, src => src.StaffRecordDocuments.Select(p=>p.FileUrl).ToList())
                           ;

            TypeAdapterConfig<StaffRecordCreateOrEdit, StaffRecord>.NewConfig()
                           .Map(dest => dest.RecordType, src => (StaffRecordType)src.RecordType)
                           .Map(dest => dest.RecordDetailType, src => (StaffRecordDetailType)src.RecordDetailType)
                           .Map(dest => dest.StaffRecordDocuments, src => src.StaffRecordDocuments.Select(p => new StaffRecordDocument { FileUrl = p }).ToList())
                           ;

            TypeAdapterConfig<StaffRecord, StaffRecordList>.NewConfig()
                            .Map(dest => dest.EmployeeName, src => src.Employee!=null? src.Employee.Name : "")
                            .Map(dest => dest.EmployeeCode, src => src.Employee != null ? src.Employee.EmployeeCode : "")
                           ;

            TypeAdapterConfig<StaffRecord, LeaveHistoryList>.NewConfig()
                            .Map(dest => dest.EmployeeName, src => src.Employee != null ? src.Employee.Name : "")
                            .Map(dest => dest.EmployeeCode, src => src.Employee != null ? src.Employee.EmployeeCode : "")
                            .Map(dest => dest.BrandEmployees, src => src.Employee != null ? src.Employee.BrandEmployees.Select(p =>p.BrandId).ToList() : new List<int>())
                           ;

            TypeAdapterConfig<User, EmployeeList>.NewConfig()
                            .Map(dest => dest.BrandIds, src => src.BrandEmployees.Select(p=>p.BrandId))
                           ;
        }
    }
}
