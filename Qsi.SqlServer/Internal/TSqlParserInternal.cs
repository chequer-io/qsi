using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Qsi.Parsing;
using Qsi.SqlServer.Common;

namespace Qsi.SqlServer.Internal
{
    internal sealed class TSqlParserInternal
    {
        private readonly TSqlParser _parser;

        public TSqlParserInternal(TransactSqlVersion tsqlParserVersion, bool initialQuotedIdentifiers)
        {
            switch (tsqlParserVersion)
            {
                case TransactSqlVersion.Version80:
                    _parser = new TSql80Parser(initialQuotedIdentifiers);
                    break;

                case TransactSqlVersion.Version90:
                    _parser = new TSql90Parser(initialQuotedIdentifiers);
                    break;

                case TransactSqlVersion.Version100:
                    _parser = new TSql100Parser(initialQuotedIdentifiers);
                    break;

                case TransactSqlVersion.Version110:
                    _parser = new TSql110Parser(initialQuotedIdentifiers);
                    break;

                case TransactSqlVersion.Version120:
                    _parser = new TSql120Parser(initialQuotedIdentifiers);
                    break;

                case TransactSqlVersion.Version130:
                    _parser = new TSql130Parser(initialQuotedIdentifiers);
                    break;

                case TransactSqlVersion.Version140:
                    _parser = new TSql140Parser(initialQuotedIdentifiers);
                    break;

                case TransactSqlVersion.Version150:
                    _parser = new TSql150Parser(initialQuotedIdentifiers);
                    break;

                default:
                    throw new ArgumentException(nameof(tsqlParserVersion));
            }
        }

        public TSqlFragment Parse(string input)
        {
            using var reader = new StringReader(input);
            var result = _parser.Parse(reader, out IList<ParseError> errors);

            QsiSyntaxErrorException[] syntaxErrors = errors
                .Select(error => new QsiSyntaxErrorException(error.Line, error.Column, error.Message))
                .ToArray();

            if (syntaxErrors.Length == 1)
                throw syntaxErrors[0];

            if (syntaxErrors.Length > 1)
                throw new AggregateException(syntaxErrors.Cast<Exception>());

            return result;
        }
    }
}
