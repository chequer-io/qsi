using System;
using System.Threading;
using Qsi.PostgreSql.Tree;

namespace Qsi.PostgreSql.Internal
{
    internal interface IPgParser : IDisposable
    {
        IPgNode Parse(string input, CancellationToken token);

        IPgVisitorSet CreateVisitorSet();
    }
}
