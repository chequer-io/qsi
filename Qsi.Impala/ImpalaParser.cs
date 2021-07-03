using System;
using System.Threading;
using Antlr4.Runtime;
using Qsi.Data;
using Qsi.Impala.Internal;
using Qsi.Parsing;
using Qsi.Tree;

namespace Qsi.Impala
{
    public sealed class ImpalaParser : IQsiTreeParser
    {
        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
