namespace MGDistributedLoggingSystem.Constants
{
    public static class Permissions
    {
        public static List<string> GeneratePermissionsForModule(string module)
        {
            return new List<string>
            {
                $"Permissions.{module}.Create",
                $"Permissions.{module}.View",
                $"Permissions.{module}.Edit",
                $"Permissions.{module}.Delete",
            };
        }
        public static List<string> GenerateAllPermissions()
        {
            var allPermissions = new List<string>();
            var allModules = Enum.GetValues(typeof(Modules)).Cast<string>().ToList();
            foreach (var module in allModules)
            {
                allPermissions.AddRange(GeneratePermissionsForModule(module));
            }
            return allPermissions;
        }
        public static class Admin
        {
            public const string Create = "Permissions.Admin.Create";
            public const string View = "Permissions.Admin.View";
            public const string Edit = "Permissions.Admin.Edit";
            public const string Delete = "Permissions.Admin.Delete";
        }
        public static class User
        {
            public const string Create = "Permissions.User.Create";
            public const string View = "Permissions.User.View";
            public const string Edit = "Permissions.User.Edit";
            public const string Delete = "Permissions.User.Delete";
        }

    }
}
