using System;
using System.Linq;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using Qsi.Parsing;
using Qsi.SqlServer.Common;
using ManagementTransactSqlVersion = Microsoft.SqlServer.Management.SqlParser.Common.TransactSqlVersion;

namespace Qsi.SqlServer.Internal;

internal sealed class AlternativeParserInternal
{
    private readonly ParseOptions _parserOptions;

    public AlternativeParserInternal(TransactSqlVersion tsqlParserVersion)
    {
        _parserOptions = new ParseOptions
        {
            TransactSqlVersion = tsqlParserVersion switch
            {
                TransactSqlVersion.Version80 => ManagementTransactSqlVersion.Version105,
                TransactSqlVersion.Version90 => ManagementTransactSqlVersion.Version105,
                TransactSqlVersion.Version100 => ManagementTransactSqlVersion.Version105,
                TransactSqlVersion.Version110 => ManagementTransactSqlVersion.Version110,
                TransactSqlVersion.Version120 => ManagementTransactSqlVersion.Version120,
                TransactSqlVersion.Version130 => ManagementTransactSqlVersion.Version130,
                TransactSqlVersion.Version140 => ManagementTransactSqlVersion.Version140,
                TransactSqlVersion.Version150 => ManagementTransactSqlVersion.Version150,
                _ => ManagementTransactSqlVersion.Version160
            }
        };
    }

    public bool TryParse(string input, out SqlCodeObject result)
    {
        try
        {
            result = Parse(input);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    public SqlCodeObject Parse(string input)
    {
        var result = Parser.Parse(input, _parserOptions);

        QsiSyntaxErrorException[] syntaxErrors = result.Errors
            .Select(error => new QsiSyntaxErrorException(error.Start.LineNumber, error.Start.ColumnNumber, error.Message))
            .ToArray();

        switch (syntaxErrors.Length)
        {
            case 1:
                throw syntaxErrors[0];

            case > 1:
                throw new AggregateException(syntaxErrors.Cast<Exception>());
        }

        return result.Script;
    }
}
