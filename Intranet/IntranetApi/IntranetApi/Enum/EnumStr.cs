namespace IntranetApi.Enum
{
    public static class SortDirection
    {
        public const string ASC = "asc";
        public const string DESC = "desc";
    }

    public static class StatusValue
    {
        public const string Active = "Active";
        public const string Inactive = "Inactive";
    }

    public static class TableName
    {
        public const string Employee = "Employee";
    }
    public static class BrandValue
    {
        public const string AllBrands = "All Brands";
    }
    public static class CacheKeys
    {
        public const string GetRolesDropdown = "GetRolesDropdown";
        public const string GetRanksDropdown = "GetRanksDropdown";
        public const string GetBanksDropdown = "GetBanksDropdown";
        public const string GetBrandsDropdown = "GetBrandsDropdown";
        public const string GetDepartmentsDropdown = "GetDepartmentsDropdown";
        public const string GetAdminUserDropdown = "GetAdminUserDropdown";
    }

    public static class StoredProcedureName
    {
        public const string GetRoleList = "SP_Filter_Role";
        public const string GetRoleTotal = "SP_Filter_Role_CountTotal";

        public const string GetBankList = "SP_Filter_Bank";
        public const string GetBankTotal = "SP_Filter_Bank_CountTotal";

        public const string GetBrandList = "SP_Filter_Brand";
        public const string GetBrandTotal = "SP_Filter_Brand_CountTotal";

        public const string GetDepartmentList = "SP_Filter_Department";
        public const string GetDepartmentTotal = "SP_Filter_Department_CountTotal";

        public const string GetEmployeeList = "SP_Filter_Employee";
        public const string GetEmployeeTotal = "SP_Filter_Employee_CountTotal";
    }
}
