using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.SqlServer.Tree
{
    internal sealed class DefinitionVisitor : VisitorBase
    {
        public DefinitionVisitor(IVisitorContext visitorContext) : base(visitorContext)
        {
        }

        public IQsiDefinitionNode VisitViewStatementBody(ViewStatementBody viewStatementBody)
        {
            if (viewStatementBody is not (CreateViewStatement or CreateOrAlterViewStatement))
                throw TreeHelper.NotSupportedTree(viewStatementBody);

            var node = new SqlServerViewDefinitionNode
            {
                IsAlter = viewStatementBody is CreateOrAlterViewStatement,
                IsMaterialiazed = viewStatementBody.IsMaterialized,
                WithCheckOption = viewStatementBody.WithCheckOption,
                ViewOptions = viewStatementBody.ViewOptions?.Select(option => option.OptionKind.ToString()).ToArray(),
                Identifier = IdentifierVisitor.CreateQualifiedIdentifier(viewStatementBody.SchemaObjectName)
            };

            if (ListUtility.IsNullOrEmpty(viewStatementBody.Columns))
            {
                node.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
            }
            else
            {
                var columnsDeclaration = new QsiColumnsDeclarationNode();
                columnsDeclaration.Columns.AddRange(TableVisitor.CreateSequentialColumnNodes(viewStatementBody.Columns));
                node.Columns.SetValue(columnsDeclaration);
            }

            node.Source.SetValue(TableVisitor.VisitSelectStatement(viewStatementBody.SelectStatement));

            SqlServerTree.PutFragmentSpan(node, viewStatementBody);

            return node;
        }
    }
}
