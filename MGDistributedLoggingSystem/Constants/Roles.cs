using System.Reflection;

namespace MGDistributedLoggingSystem.Constants
{
    public static class Roles
    { 
        public const string Admin = nameof(Admin);
        public const string User = nameof(User);
        public static IEnumerable<string> ToList()
        {
            var t = typeof(Roles);
            var fields = typeof(Roles)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            var result = fields
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(x => (string)x.GetRawConstantValue()).ToList();

            return result;
        }
        public static string ToString()
        {
            var result = string.Join(',', ToList());

            return result;
        }

    }

}
