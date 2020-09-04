using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.SqlServer.Management.SqlParser.Common;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using Qsi.Compiler;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.SqlServer
{
    public abstract class SqlServerLanguageServiceBase : QsiLanguageServiceBase
    {
        public ParseOptions ParseOptions { get; }

        protected SqlServerLanguageServiceBase(DatabaseCompatibilityLevel compatibilityLevel)
        {
            ParseOptions = new ParseOptions
            {
                CompatibilityLevel = compatibilityLevel
            };
        }

        protected SqlServerLanguageServiceBase([NotNull] ParseOptions options)
        {
            ParseOptions = options ?? throw new ArgumentNullException(nameof(options));
        }

        public override IQsiTreeParser CreateTreeParser()
        {
            return new SqlServerParser(ParseOptions);
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new SqlServerScriptParser(ParseOptions);
        }

        public override QsiTableCompileOptions CreateCompileOptions()
        {
            return new QsiTableCompileOptions
            {
                AllowEmptyColumnsInSelect = false
            };
        }
    }
}
