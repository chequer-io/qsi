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
    public class SqlServerParser : IQsiTreeParser
    {
        public event EventHandler<QsiSyntaxErrorException> SyntaxError;

        private readonly ParseOptions _options;

        public SqlServerParser(ParseOptions options)
        {
            _options = options;
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
