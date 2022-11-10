using System.Threading;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.PostgreSql.Internal;
using Qsi.PostgreSql.Tree.Visitors;
using Qsi.Tree;
using Qsi.Utilities;

using static Qsi.PostgreSql.Internal.PostgreSqlParserInternal;

namespace Qsi.PostgreSql
{
    public class PostgreSqlParser : IQsiTreeParser
    {
        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            var parser = PostgreSqlUtility.CreateParser(script.Script);
            var root = parser.root();

            if (root.children[0] is not StatementContext statement)
            {
                return null;
            }

            return statement.children[0] switch
            {
                SelectStatementContext select => TableVisitor.VisitSelectStatement(select),
                InsertStatementContext insert => ActionVisitor.VisitInsertStatement(insert),
                UpdateStatementContext update => ActionVisitor.VisitUpdateStatement(update),
                DeleteStatementContext delete => ActionVisitor.VisitDeleteStatement(delete),
                CreateStatementContext create => DefinitionVisitor.VisitCreateStatement(create),
                _ => throw TreeHelper.NotSupportedTree(statement.children[0])
            };
        }
    }
}
