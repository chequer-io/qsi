using System;
using System.Threading;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.Services;
using Qsi.Tree;

namespace Qsi.Athena
{
    public class AthenaParser : IQsiTreeParser
    {
        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
