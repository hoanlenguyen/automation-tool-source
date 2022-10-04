using IntranetApi.Models.Permission;
using Microsoft.AspNetCore.Authorization;

namespace IntranetApi.Models
{
    public static class Permissions
    {
        public const string Bank = "Bank";
        public const string Brand = "Brand";
        public const string Department = "Department";
        public const string Rank = "Rank";
        public const string Role = "Role";
        public const string Currency = "Currency";
        public const string StaffRecord = "StaffRecord";
        public const string LeaveHistory = "LeaveHistory";
        public const string Employee = "Employee";

        public const string Type = "Permission";
        public static List<string> GeneratePermissionsForModule(string module)
        {
            return new List<string>()
            {
                $"{module}.Create",
                $"{module}.View",
                $"{module}.Update",
                $"{module}.Delete"
            };
        }

        public static void AddCustomizedAuthorizationOptions(this AuthorizationOptions option, params string[] modules)
        {
            foreach (var module in modules)
            {
                var allPermissions = GeneratePermissionsForModule(module);
                foreach (var permission in allPermissions)
                {
                    option.AddPolicy(permission, policy => policy.Requirements.Add(new PermissionRequirement(permission)));
                }
            }
        }        
    }

    public static class BankPermissions
    {
        public const string View = "Bank.View";
        public const string Create = "Bank.Create";
        public const string Update = "Bank.Update";
        public const string Delete = "Bank.Delete";
    }

    public static class BrandPermissions
    {
        public const string View = "Brand.View";
        public const string Create = "Brand.Create";
        public const string Update = "Brand.Update";
        public const string Delete = "Brand.Delete";
    }

    public static class RankPermissions
    {
        public const string View = "Rank.View";
        public const string Create = "Rank.Create";
        public const string Update = "Rank.Update";
        public const string Delete = "Rank.Delete";
    }

    public static class DepartmentPermissions
    {
        public const string View = "Department.View";
        public const string Create = "Department.Create";
        public const string Update = "Department.Update";
        public const string Delete = "Department.Delete";
    }


    public static class EmployeePermissions
    {
        public const string View = "Employee.View";
        public const string Create = "Employee.Create";
        public const string Update = "Employee.Update";
        public const string Delete = "Employee.Delete";
    }

    public static class RolePermissions
    {
        public const string View = "Role.View";
        public const string Create = "Role.Create";
        public const string Update = "Role.Update";
        public const string Delete = "Role.Delete";
    }

    public static class StaffRecordPermissions
    {
        public const string View = "StaffRecord.View";
        public const string Create = "StaffRecord.Create";
        public const string Update = "StaffRecord.Update";
        public const string Delete = "StaffRecord.Delete";
    }

    public static class LeaveHistoryPermissions
    {
        public const string View = "LeaveHistory.View";
        public const string Create = "LeaveHistory.Create";
        //public const string Update = "StaffRecord.Update";
        //public const string Delete = "StaffRecord.Delete";
    }

    public static class CurrencyPermissions
    {
        public const string View = "Currency.View";
        public const string Create = "Currency.Create";
        public const string Update = "Currency.Update";
        public const string Delete = "Currency.Delete";
    }
}