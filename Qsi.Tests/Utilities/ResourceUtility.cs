using System.IO;
using System.Reflection;

namespace Qsi.Tests.Utilities;

public class ResourceUtility
{
    public static Stream GetResourceStream(string resource)
    {
        var stream = Assembly.GetCallingAssembly().GetManifestResourceStream($"Qsi.Tests.Resources.{resource}");
        return stream;
    }

    public static string GetResourceContent(string resource)
    {
        var stream = Assembly.GetCallingAssembly().GetManifestResourceStream($"Qsi.Tests.Resources.{resource}");
        var reader = new StreamReader(stream!);

        return reader.ReadToEnd();
    }
}
