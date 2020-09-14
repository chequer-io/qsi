using System;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.SqlServer.Tree;
using Qsi.Tree;

namespace Qsi.SqlServer
{
    public sealed class SqlServerParser : IQsiTreeParser, IContext
    {
        public event EventHandler<QsiSyntaxErrorException> SyntaxError;

        private readonly ParseOptions _options;

        public TableVisitor_Legacy TableVisitor { get; }

        public ExpressionVisitor_Legacy ExpressionVisitor { get; }

        public IdentifierVisitor_Legacy IdentifierVisitor { get; }

        public SqlServerParser SqlParser => this;

        public SqlServerParser(ParseOptions options)
        {
            _options = options;
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
            var result = Parser.Parse(script.Script, _options);
            
            Error[] errors = result.Errors
                .Where(e => !e.IsWarning)
                .ToArray();

            if (errors.Length > 0)
            {
                var message = new StringBuilder();

                foreach (var error in errors)
                {
                    if (message.Length > 0)
                        message.AppendLine();

                    message.Append(error.Message);
                }

                SyntaxError?.Invoke(this, new QsiSyntaxErrorException(0, 0, message.ToString()));
                return null;
            }

            var statement = result
                .Script.Batches.FirstOrDefault()?
                .Statements.FirstOrDefault() ?? throw new QsiException(QsiError.Syntax);
            
            return TableVisitor.Visit(statement);
        }
    }
}
