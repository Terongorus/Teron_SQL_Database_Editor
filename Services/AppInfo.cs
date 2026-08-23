using System.Reflection;

namespace Logic
{
    internal static class AppInfo
    {
        public static string DisplayName
        {
            get
            {
                string product = Assembly.GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "TeronSQLDatabaseEditor";
                int abbreviationStart = product.IndexOf(" (");
                return abbreviationStart >= 0 ? product[..abbreviationStart] : product;
            }
        }

        public static string Version =>
            Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.0.0.0";

        public static string DisplayNameWithVersion => $"{DisplayName} v{Version}";
    }
}
