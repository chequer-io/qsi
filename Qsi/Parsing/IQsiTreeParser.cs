using System.Threading;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Parsing;

public interface IQsiTreeParser
{
    IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default);
}