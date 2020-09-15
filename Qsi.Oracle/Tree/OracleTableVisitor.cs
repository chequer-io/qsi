using net.sf.jsqlparser.schema;
using Qsi.JSql.Tree;
using Qsi.Tree.Base;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree
{
    internal sealed class OracleTableVisitor : JSqlTableVisitor
    {
        public OracleTableVisitor(IJSqlVisitorContext context) : base(context)
        {
        }

        public override QsiColumnNode VisitColumn(Column column)
        {
            var columnNode = base.VisitColumn(column);

            if (columnNode is QsiDeclaredColumnNode declaredColumn &&
                declaredColumn.Name.Level == 1 && !declaredColumn.Name[0].IsEscaped &&
                OracleFunction.Contains(declaredColumn.Name[0].Value))
            {
                return TreeHelper.Create<QsiDerivedColumnNode>(cn =>
                {
                    cn.Expression.SetValue(TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(new QsiFunctionAccessExpressionNode
                        {
                            Identifier = declaredColumn.Name
                        });
                    }));

                    cn.Alias.SetValue(new QsiAliasNode
                    {
                        Name = declaredColumn.Name[0]
                    });
                });
            }

            return columnNode;
        }
    }
}
