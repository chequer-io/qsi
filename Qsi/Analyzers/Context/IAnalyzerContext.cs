using System.Threading;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Analyzers.Context
{
    public interface IAnalyzerContext
    {
        QsiEngine Engine { get; }

        QsiScript Script { get; }

        IQsiTreeNode Tree { get; }

        QsiAnalyzerOptions Options { get; }

        CancellationToken CancellationToken { get; }
    }
}
