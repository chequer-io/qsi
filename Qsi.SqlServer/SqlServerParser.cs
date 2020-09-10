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
    public sealed class SqlServerParser : IQsiTreeParser, IVisitorContext
    {
        public event EventHandler<QsiSyntaxErrorException> SyntaxError;

        private readonly ParseOptions _options;

        public TableVisitor TableVisitor { get; }

        public ExpressionVisitor ExpressionVisitor { get; }

        public IdentifierVisitor IdentifierVisitor { get; }
        
        public SqlServerParser(ParseOptions options)
        {
            _options = options;
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
