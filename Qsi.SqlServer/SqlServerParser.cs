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
    public sealed class SqlServerParser : IQsiTreeParser, IContext
    {
        public TableVisitor TableVisitor { get; }

        public ExpressionVisitor ExpressionVisitor { get; }

        public IdentifierVisitor IdentifierVisitor { get; }

        public SqlServerParser SqlParser => this;

        private readonly TSqlParserInternal _parser;

        public SqlServerParser(TransactSqlVersion transactSqlVersion)
        {
            _parser = new TSqlParserInternal(transactSqlVersion, false);
            TableVisitor = CreateTableVisitor();
            ExpressionVisitor = CreateExpressionVisitor();
            IdentifierVisitor = CreateIdentifierVisitor();
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
                return TableVisitor.Visit(sqlScript.Batches.FirstOrDefault());
            }
            
            throw new InvalidOperationException();
        }
    }
}
