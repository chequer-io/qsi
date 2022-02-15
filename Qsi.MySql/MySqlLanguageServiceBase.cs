using System;
using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Definition;
using Qsi.Collections;
using Qsi.Data;
using Qsi.Engines;
using Qsi.MySql.Analyzers;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.MySql
{
    public abstract class MySqlLanguageServiceBase : QsiLanguageServiceBase
    {
        public abstract Version Version { get; }

        protected override IEqualityComparer<QsiIdentifier> GetIdentifierComparer()
        {
            return new QsiIdentifierEqualityComparer(StringComparison.OrdinalIgnoreCase);
        }

        public override IQsiTreeParser CreateTreeParser()
        {
            return new MySqlParser(Version);
        }

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            return new MySqlDeparser();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new MySqlScriptParser();
        }

        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new QsiAnalyzerOptions
            {
                AllowEmptyColumnsInSelect = false
            };
        }

        public override IEnumerable<QsiAnalyzerBase> CreateAnalyzers(QsiEngine engine)
        {
            yield return new QsiActionAnalyzer(engine);
            yield return new MySqlTableAnalyzer(engine);
            yield return new QsiDefinitionAnalyzer(engine);
        }
    }
}
