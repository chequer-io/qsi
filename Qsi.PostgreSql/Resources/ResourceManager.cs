using System.IO;
using System.Reflection;

namespace Qsi.PostgreSql.Resources;

internal static class ResourceManager
{
    private static readonly Assembly _assembly;

    static ResourceManager()
    {
        _assembly = Assembly.GetCallingAssembly();
    }

    public static string GetResourceContent(string resource)
    {
        var stream = _assembly.GetManifestResourceStream($"Qsi.PostgreSql.Resources.{resource}");
        var reader = new StreamReader(stream!);

        return reader.ReadToEnd();
    }
}