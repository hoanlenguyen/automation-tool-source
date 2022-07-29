namespace IntranetApi.Models
{
    public static class Permissions
    {
        public static List<string> GeneratePermissionsForModule(string module)
        {
            return new List<string>()
            {
                $"Permissions.{module}.Create",
                $"Permissions.{module}.View",
                $"Permissions.{module}.Edit",
                $"Permissions.{module}.Delete",
            };
        }

        public static class BankPermissions
        {
            public const string View = "Permissions.Banks.View";
            public const string Create = "Permissions.Banks.Create";
            public const string Edit = "Permissions.Banks.Edit";
            public const string Delete = "Permissions.Banks.Delete";
        }
    }
}