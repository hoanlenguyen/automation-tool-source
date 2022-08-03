using IntranetApi.Models.Permission;
using Microsoft.AspNetCore.Authorization;

namespace IntranetApi.Models
{
    public static class Permissions
    {
        public const string Type = "Permission";
        public static List<string> GeneratePermissionsForModule(string module)
        {
            return new List<string>()
            {
                $"Permissions.{module}.Create",
                $"Permissions.{module}.View",
                $"Permissions.{module}.Update",
                $"Permissions.{module}.Delete",
            };
        }

        public static void AddCustomizedAuthorizationOptions(this AuthorizationOptions option, params string[] modules)
        {
            foreach (var module in modules)
            {
                var allPermissions = GeneratePermissionsForModule(module);
                foreach (var permission in allPermissions)
                {
                    //Console.WriteLine(permission);
                    option.AddPolicy(permission, policy => policy.Requirements.Add(new PermissionRequirement(permission)));
                }
            }
        }        
    }

    public static class BankPermissions
    {
        public const string View = "Permissions.Bank.View";
        public const string Create = "Permissions.Bank.Create";
        public const string Update = "Permissions.Bank.Update";
        public const string Delete = "Permissions.Bank.Delete";
    }

    public static class BrandPermissions
    {
        public const string View = "Permissions.Brand.View";
        public const string Create = "Permissions.Brand.Create";
        public const string Update = "Permissions.Brand.Update";
        public const string Delete = "Permissions.Brand.Delete";
    }

    public static class RankPermissions
    {
        public const string View = "Permissions.Rank.View";
        public const string Create = "Permissions.Rank.Create";
        public const string Update = "Permissions.Rank.Update";
        public const string Delete = "Permissions.Rank.Delete";
    }

    public static class DepartmentPermissions
    {
        public const string View = "Permissions.Department.View";
        public const string Create = "Permissions.Department.Create";
        public const string Update = "Permissions.Department.Update";
        public const string Delete = "Permissions.Department.Delete";
    }


    public static class EmployeePermissions
    {
        public const string View = "Permissions.Employee.View";
        public const string Create = "Permissions.Employee.Create";
        public const string Update = "Permissions.Employee.Update";
        public const string Delete = "Permissions.Employee.Delete";
    }

    public static class RolePermissions
    {
        public const string View = "Permissions.Role.View";
        public const string Create = "Permissions.Role.Create";
        public const string Update = "Permissions.Role.Update";
        public const string Delete = "Permissions.Role.Delete";
    }
}