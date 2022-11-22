using System;
using Qsi.PostgreSql.Tree;

namespace Qsi.PostgreSql.Internal
{
    internal interface IPgParser : IDisposable
    {
        IPgNode Parse(string input);

        IPgVisitorSet CreateVisitorSet();
    }
}
