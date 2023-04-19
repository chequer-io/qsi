using System;
using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Definition;
using Qsi.Engines;
using Qsi.Parsing;
using Qsi.PostgreSql.Analyzers;
using Qsi.Services;

namespace Qsi.PostgreSql
{
    public abstract class PostgreSqlLanguageServiceBase : QsiLanguageServiceBase
    {
        public int TotalStack { get; set; } = 1024 * 1024 * 25; // 25MB

        public ulong TotalMemory { get; set; } = 1024 * 1024 * 1024; // 1GB

        public override IQsiTreeParser CreateTreeParser()
        {
            return new PostgreSqlParser(TotalStack, TotalMemory);
        }

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            return new PostgreSqlDeparser();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new PostgreSqlScriptParser();
        }

        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new()
            {
                AllowEmptyColumnsInSelect = true,
                AllowEmptyColumnsInInline = true,
                AllowNoAliasInDerivedTable = true
            };
        }

        public override IEnumerable<IQsiAnalyzer> CreateAnalyzers(QsiEngine engine)
        {
            yield return new QsiActionAnalyzer(engine);
            yield return new PgTableAnalyzer(engine);
            yield return new QsiDefinitionAnalyzer(engine);
        }
    }
}
