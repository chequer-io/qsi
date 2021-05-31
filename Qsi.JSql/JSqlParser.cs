using System.Threading;
using Qsi.Data;
using Qsi.JSql.Internal;
using Qsi.JSql.Tree;
using Qsi.Parsing;
using Qsi.Tree;

namespace Qsi.JSql
{
    public class JSqlParser : IQsiTreeParser
    {
        protected virtual JSqlTableVisitor CreateTableVisitor(IJSqlVisitorSet set)
        {
            return new(set);
        }

        protected virtual JSqlExpressionVisitor CreateExpressionVisitor(IJSqlVisitorSet set)
        {
            return new(set);
        }

        protected virtual JSqlIdentifierVisitor CreateIdentifierVisitor(IJSqlVisitorSet set)
        {
            return new(set);
        }

        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            var statement = CCJSqlParserUtility.Parse(script.Script);
            var visitorSet = new VisitorSetImpl();

            visitorSet.TableVisitor = CreateTableVisitor(visitorSet);
            visitorSet.ExpressionVisitor = CreateExpressionVisitor(visitorSet);
            visitorSet.IdentifierVisitor = CreateIdentifierVisitor(visitorSet);

            return visitorSet.TableVisitor.Visit(statement) ?? throw new QsiException(QsiError.NotSupportedScript, script.ScriptType);
        }

        private sealed class VisitorSetImpl : IJSqlVisitorSet
        {
            public JSqlTableVisitor TableVisitor { get; set; }

            public JSqlExpressionVisitor ExpressionVisitor { get; set; }

            public JSqlIdentifierVisitor IdentifierVisitor { get; set; }
        }
    }
}
