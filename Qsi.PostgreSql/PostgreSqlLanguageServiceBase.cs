using System;
using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Definition;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Parsing;
using Qsi.PostgreSql.Analyzers;
using Qsi.Services;
using Qsi.Tree;

namespace Qsi.PostgreSql
{
    public abstract class PostgreSqlLanguageServiceBase : QsiLanguageServiceBase
    {
        public override IQsiTreeParser CreateTreeParser()
        {
            return new PostgreSqlParser();
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
            // TODO: Check table analyzer.
            // yield return new PgTableAnalyzer(engine);
            yield return new PostgreSqlTableAnalyzer(engine);
            yield return new QsiDefinitionAnalyzer(engine);
        }
    }
}
