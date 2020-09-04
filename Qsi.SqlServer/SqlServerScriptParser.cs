using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using Qsi.Data;
using Qsi.Parsing;

namespace Qsi.SqlServer
{
    public class SqlServerScriptParser : IQsiScriptParser
    {
        private readonly ParseOptions _parseOptions;

        public SqlServerScriptParser(ParseOptions options)
        {
            _parseOptions = options;
        }

        public IEnumerable<QsiScript> Parse(in string input)
        {
            return ParseInternal(input).ToArray();
        }

        private IEnumerable<QsiScript> ParseInternal(string input)
        {
            var result = Parser.Parse(input, _parseOptions);

            foreach (var statement in result.Script.Batches.SelectMany(b => b.Statements))
            {
                yield return new QsiScript(statement.Sql, GetStatementType(statement));
            }
        }

        private QsiScriptType GetStatementType(SqlStatement statement)
        {
            switch (statement)
            {
                case SqlSelectStatement _:
                    return QsiScriptType.Select;

                case SqlInsertStatement _:
                    return QsiScriptType.Insert;

                case SqlUpdateStatement _:
                    return QsiScriptType.Update;

                case SqlDeleteStatement _:
                    return QsiScriptType.Delete;

                case SqlCreateTableStatement _:
                    return QsiScriptType.CreateTable;

                case SqlCreateViewStatement _:
                    return QsiScriptType.CreateView;

                case SqlCreateFunctionStatement _:
                    return QsiScriptType.CreateFunction;

                case SqlCreateProcedureStatement _:
                    return QsiScriptType.CreateProcedure;

                case SqlAlterViewStatement _:
                    return QsiScriptType.AlterView;

                case SqlAlterFunctionStatement _:
                    return QsiScriptType.AlterFunction;

                case SqlAlterProcedureStatement _:
                    return QsiScriptType.AlterProcedure;

                case SqlDropTableStatement _:
                    return QsiScriptType.DropTable;

                case SqlDropViewStatement _:
                    return QsiScriptType.DropView;

                case SqlDropFunctionStatement _:
                    return QsiScriptType.DropFunction;

                case SqlDropProcedureStatement _:
                    return QsiScriptType.DropProcedure;
            }

            return QsiScriptType.Unknown;
        }
    }
}
