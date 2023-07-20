using Qsi.Analyzers;

namespace Qsi.Shared.Extensions;

internal static class IQsiAnalysisResultExtension
{
    public static IQsiAnalysisResult[] ToSingleArray(this IQsiAnalysisResult result)
    {
        return new[] { result };
    }
}