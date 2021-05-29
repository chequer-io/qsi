using net.sf.jsqlparser.schema;
using net.sf.jsqlparser.statement.@select;
using Qsi.JSql.Tree;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree
{
    internal sealed class OracleTableVisitor : JSqlTableVisitor
    {
        public OracleTableVisitor(IJSqlVisitorSet set) : base(set)
        {
        }

        public override QsiColumnNode VisitColumn(Column column)
        {
            var columnNode = base.VisitColumn(column);

            if (columnNode is QsiColumnReferenceNode columnReferenceNode &&
                columnReferenceNode.Name.Level == 1 && !columnReferenceNode.Name[0].IsEscaped &&
                OracleFunction.Contains(columnReferenceNode.Name[0].Value))
            {
                return TreeHelper.Create<QsiDerivedColumnNode>(cn =>
                {
                    cn.Expression.SetValue(TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(new QsiFunctionExpressionNode
                        {
                            Identifier = columnReferenceNode.Name
                        });
                    }));

                    cn.Alias.SetValue(new QsiAliasNode
                    {
                        Name = columnReferenceNode.Name[0]
                    });
                });
            }

            return columnNode;
        }

        public override QsiTableNode VisitSelect(Select select)
        {
            var tableNode = base.VisitSelect(select);

            if (tableNode is QsiDerivedTableNode derivedTableNode && !derivedTableNode.Directives.IsEmpty)
                derivedTableNode.Directives.Value.IsRecursive = true;

            return tableNode;
        }

        public override QsiTableNode VisitFromItem(FromItem item)
        {
            if (item.getAlias()?.getAliasColumns()?.size() > 0)
            {
                throw new QsiException(QsiError.Syntax);
            }

            return base.VisitFromItem(item);
        }

        public override QsiTableNode VisitValuesList(ValuesList valuesList)
        {
            throw new QsiException(QsiError.Syntax);
        }
    }
}
