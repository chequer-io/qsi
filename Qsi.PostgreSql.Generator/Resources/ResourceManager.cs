using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Qsi.PostgreSql.Generator.Resources
{
    internal static class ResourceManager
    {
        private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
        private static readonly HashSet<string> _resourceNames = new HashSet<string>(_assembly.GetManifestResourceNames());

        public static bool HasResource(string resourceName)
        {
            return _resourceNames.Contains($"Qsi.PostgreSql.Generator.Resources.{resourceName}");
        }
        
        public static Stream FindResource(string resourceName)
        {
            var stream = _assembly.GetManifestResourceStream($"Qsi.PostgreSql.Generator.Resources.{resourceName}");
            return stream ?? throw new KeyNotFoundException();
        }

        public static IEnumerable<string> FindChildResourceNames(string resourceName, StringComparison comparison = StringComparison.Ordinal)
        {
            var path = $"Qsi.PostgreSql.Generator.Resources.{resourceName}";

            foreach (var name in _resourceNames.Where(name => name.StartsWith(path, comparison)))
            {
                yield return name.Substring(path.Length - resourceName.Length);
            }
        }

        public static string FindStringResource(string resourceName)
        {
            using var stream = FindResource(resourceName);
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }

        public static T FindJsonResource<T>(string resourceName)
        {
            string json = FindStringResource(resourceName);
            return JsonConvert.DeserializeObject<T>(json);
        }
        
        public static object FindJsonResource(string resourceName, Type type)
        {
            string json = FindStringResource(resourceName);
            return JsonConvert.DeserializeObject(json, type);
        }
    }
}
