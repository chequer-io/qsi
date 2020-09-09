using System;
using net.sf.jsqlparser;
using net.sf.jsqlparser.parser;
using Qsi.Data;
using Qsi.JSql.Extensions;
using Qsi.JSql.Tree;
using Qsi.Parsing;
using Qsi.Tree;

namespace Qsi.JSql
{
    public class JSqlParser : IQsiTreeParser, IJSqlVisitorContext
    {
        public event EventHandler<QsiSyntaxErrorException> SyntaxError;

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

        public IQsiTreeNode Parse(QsiScript script)
        {
            try
            {
                var statement = CCJSqlParserUtil.parse(script.Script);
                return TableVisitor.Visit(statement);
            }
            catch (JSQLParserException e)
            {
                SyntaxError?.Invoke(this, e.AsSyntaxError());
            }

            return null;
        }
    }
}
