using System;
using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Collections;
using Qsi.Data;
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
            return new MySqlScriptParser(Version);
        }

        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new()
            {
                AllowEmptyColumnsInSelect = false
            };
        }

        public override IEnumerable<QsiAnalyzerBase> CreateAnalyzers(QsiEngine engine)
        {
            yield return new QsiActionAnalyzer(engine);
            yield return new MySqlTableAnalyzer(engine);
        }
    }
}
