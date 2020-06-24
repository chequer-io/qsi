using System;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Parsing
{
    public interface IQsiParser
    {
        event EventHandler<QsiSyntaxErrorException> SyntaxError;

        IQsiTreeNode Parse(QsiScript script);
    }
}
