using System;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.SqlServer.Common;
using Qsi.SqlServer.Internal;
using Qsi.SqlServer.Tree;
using Qsi.Tree;

namespace Qsi.SqlServer
{
    public sealed class SqlServerParser : IQsiTreeParser, IVisitorContext
    {
        #region IContext
        TableVisitor IVisitorContext.TableVisitor => _tableVisitor;

        ExpressionVisitor IVisitorContext.ExpressionVisitor => _expressionVisitor;

        IdentifierVisitor IVisitorContext.IdentifierVisitor => _identifierVisitor;

        private readonly TableVisitor _tableVisitor;
        private readonly ExpressionVisitor _expressionVisitor;
        private readonly IdentifierVisitor _identifierVisitor;
        #endregion

        private readonly TSqlParserInternal _parser;

        public SqlServerParser(TransactSqlVersion transactSqlVersion)
        {
            _parser = new TSqlParserInternal(transactSqlVersion, false);
            _tableVisitor = CreateTableVisitor();
            _expressionVisitor = CreateExpressionVisitor();
            _identifierVisitor = CreateIdentifierVisitor();
        }

        private TableVisitor CreateTableVisitor()
        {
            return new TableVisitor(this);
        }

        private ExpressionVisitor CreateExpressionVisitor()
        {
            return new ExpressionVisitor(this);
        }

        private IdentifierVisitor CreateIdentifierVisitor()
        {
            return new IdentifierVisitor(this);
        }

        public IQsiTreeNode Parse(QsiScript script)
        {
            var result = _parser.Parse(script.Script);

            if (result is TSqlScript sqlScript)
            {
                return _tableVisitor.Visit(sqlScript.Batches.FirstOrDefault());
            }

            throw new InvalidOperationException();
        }
    }
}
