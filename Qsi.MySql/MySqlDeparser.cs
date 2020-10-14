using System;
using System.Linq;
using Qsi.Data;
using Qsi.MySql.Tree;
using Qsi.Parsing;
using Qsi.Tree;

namespace Qsi.MySql
{
    public sealed class MySqlDeparser : IQsiTreeDeparser
    {
        public string Deparse(IQsiTreeNode node, QsiScript script)
        {
            var range = MySqlTree.GetSpan(node);

            if (Equals(range, default(Range)))
                return DeparseByNode(node, script);

            return script.Script[range];
        }

        private string DeparseByNode(IQsiTreeNode node, QsiScript script)
        {
            switch (node)
            {
                case IQsiColumnNode column:
                    return DeparseByColumnNode(column, script);

                case IQsiLiteralExpressionNode literalExpression:
                    return DeparseByLiteralExpression(literalExpression, script);

                case IQsiColumnsDeclarationNode columnsDeclaration:
                    return string.Join(", ", columnsDeclaration.Columns.Select(c => DeparseByColumnNode(c, script)));

                case IQsiAliasNode alias:
                    return alias.Name.ToString();
            }

            return null;
        }

        private string DeparseByLiteralExpression(IQsiLiteralExpressionNode node, QsiScript script)
        {
            return node.Value?.ToString() ?? "null";
        }

        private string DeparseByColumnNode(IQsiColumnNode node, QsiScript script)
        {
            switch (node)
            {
                case IQsiDeclaredColumnNode declaredColumn:
                    return declaredColumn.Name.ToString();

                case IQsiSequentialColumnNode sequentialColumn:
                    return DeparseByNode(sequentialColumn.Alias, script);

                case IQsiDerivedColumnNode derivedColumn:
                {
                    var sourceNode = derivedColumn.IsColumn ?
                        (IQsiTreeNode)derivedColumn.Column : derivedColumn.Expression;

                    var source = Deparse(sourceNode, script);

                    if (derivedColumn.Alias == null)
                        return source;

                    return $"{source} AS {Deparse(derivedColumn.Alias, script)}";
                }

                case IQsiAllColumnNode allColumn:
                {
                    if (allColumn.Path != null)
                        return $"{allColumn.Path}.*";

                    return "*";
                }
            }

            return null;
        }
    }
}
