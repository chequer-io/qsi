using System;
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
        public TableVisitor_Legacy TableVisitor { get; }

        public ExpressionVisitor_Legacy ExpressionVisitor { get; }

        public IdentifierVisitor_Legacy IdentifierVisitor { get; }

        public SqlServerParser SqlParser => this;

        private readonly TSqlParserInternal _parser;

        public SqlServerParser(TransactSqlVersion transactSqlVersion)
        {
            _parser = new TSqlParserInternal(transactSqlVersion, false);
            TableVisitor = CreateTableVisitor();
            ExpressionVisitor = CreateExpressionVisitor();
            IdentifierVisitor = CreateIdentifierVisitor();
        }

        private TableVisitor_Legacy CreateTableVisitor()
        {
            return new TableVisitor_Legacy(this);
        }

        private ExpressionVisitor_Legacy CreateExpressionVisitor()
        {
            return new ExpressionVisitor_Legacy(this);
        }

        private IdentifierVisitor_Legacy CreateIdentifierVisitor()
        {
            return new IdentifierVisitor_Legacy(this);
        }

        public IQsiTreeNode Parse(QsiScript script)
        {
            var result = _parser.Parse(script.Script);

            throw new NotImplementedException();
        }
    }
}
