using System.Threading;
using Qsi.Data;
using Qsi.JSql.Internal;
using Qsi.JSql.Tree;
using Qsi.Parsing;
using Qsi.Tree;

namespace Qsi.JSql
{
    public class JSqlParser : IQsiTreeParser, IJSqlVisitorContext
    {
        public JSqlTableVisitor TableVisitor { get; }

        public JSqlExpressionVisitor ExpressionVisitor { get; }

        public JSqlIdentifierVisitor IdentifierVisitor { get; }

        public JSqlParser()
        {
            TableVisitor = CreateTableVisitor();
            ExpressionVisitor = CreateExpressionVisitor();
            IdentifierVisitor = CreateIdentifierVisitor();
        }

        protected virtual JSqlTableVisitor CreateTableVisitor()
        {
            return new JSqlTableVisitor(this);
        }

        protected virtual JSqlExpressionVisitor CreateExpressionVisitor()
        {
            return new JSqlExpressionVisitor(this);
        }

        protected virtual JSqlIdentifierVisitor CreateIdentifierVisitor()
        {
            return new JSqlIdentifierVisitor(this);
        }

        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            var statement = CCJSqlParserUtility.Parse(script.Script);
            return TableVisitor.Visit(statement) ?? throw new QsiException(QsiError.NotSupportedScript, script.ScriptType);
        }
    }
}
