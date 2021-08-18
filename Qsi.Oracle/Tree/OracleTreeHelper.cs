using Antlr4.Runtime;
using Qsi.Data;

namespace Qsi.Oracle.Tree
{
    public sealed class OracleTreeHelper
    {
        public static OracleNamedParameterExpressionNode CreateNamedParameter(ParserRuleContext context, string name)
        {
            var node = OracleTree.CreateWithSpan<OracleNamedParameterExpressionNode>(context);

            node.Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(name, false));

            return node;
        }
    }
}
