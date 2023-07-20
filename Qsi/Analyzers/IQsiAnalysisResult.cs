using System.Diagnostics.CodeAnalysis;
using Qsi.Data;

namespace Qsi.Analyzers;

public interface IQsiAnalysisResult
{
    [NotNull]
    QsiSensitiveDataCollection SensitiveDataCollection { get; }
}