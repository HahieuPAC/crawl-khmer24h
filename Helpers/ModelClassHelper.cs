using System.ComponentModel;
using System.Reflection;

namespace CrawlDataWebsiteToolBasic.Helpers
{
    public static class ModelClassHelper<T>
    {
        public static List<string> GetDescriptionProperties()
        {
            // Get all property of T
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Return list Description of each property
            return props.Select(prop => typeof(T)
                .GetMember(prop.Name.ToString())
                .First()
                .GetCustomAttribute<DescriptionAttribute>()!
                    .Description).ToList();
        }
    }
}
