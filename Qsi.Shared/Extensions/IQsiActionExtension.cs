using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Data;

namespace Qsi.Shared.Extensions
{
    internal static class IQsiActionExtension
    {
        public static IQsiAnalysisResult ToResult(this IQsiAction action)
        {
            return new QsiActionAnalysisResult(action);
        }

        public static IQsiAnalysisResult ToResult<T>(this IEnumerable<T> sources) where T : IQsiAction
        {
            return new QsiActionAnalysisResult(new QsiActionSet<T>(sources));
        }
    }
}
