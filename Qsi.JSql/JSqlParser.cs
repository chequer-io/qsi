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
    public class JSqlParser : IQsiTreeParser
    {
        public event EventHandler<QsiSyntaxErrorException> SyntaxError;

        protected JSqlTableVisitor TableVisitor => _tableVisitor ??= CreateTableVisitor();

        private JSqlTableVisitor _tableVisitor;

        protected JSqlTableVisitor CreateTableVisitor()
        {
            return new JSqlTableVisitor();
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
