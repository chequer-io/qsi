using System;
using System.Linq;
using Qsi.Diagnostics;
using Qsi.SqlServer.Common;
using Qsi.SqlServer.Internal;
using Microsoft.SqlServer.Management.SqlParser.Parser;

namespace Qsi.SqlServer.Diagnostics;

public sealed class SqlServerRawTreeParser : IRawTreeParser
{
    private readonly TSqlParserInternal _parser;

    public SqlServerRawTreeParser(TransactSqlVersion version)
    {
        _parser = new TSqlParserInternal(version, false);
    }

    public IRawTree Parse(string input)
    {
        try
        {
            return SqlServerRawTreeVisitor.CreateRawTree(_parser.Parse(input));
        }
        catch (Exception)
        {
            if (input.Contains("physloc", StringComparison.InvariantCultureIgnoreCase))
            {
                var parseResult = Parser.Parse(input);

                if (!parseResult.Errors.Any())
                    return new SqlServerRawTree(parseResult.Script);
            }

            throw;
        }
    }
}