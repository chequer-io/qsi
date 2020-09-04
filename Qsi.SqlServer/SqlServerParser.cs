using System;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.Tree;

namespace Qsi.SqlServer
{
    public class SqlServerParser : IQsiTreeParser
    {
        public event EventHandler<QsiSyntaxErrorException> SyntaxError;

        public IQsiTreeNode Parse(QsiScript script)
        {
            throw new NotImplementedException();
        }
    }
}
