using System;
using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Engines;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.Oracle
{
    public abstract class OracleLanguageServiceBase : QsiLanguageServiceBase
    {
        public override IQsiTreeParser CreateTreeParser()
        {
            // return new OracleParser();
            throw new NotImplementedException();
        }

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            // return new OracleDeparser();
            throw new NotImplementedException();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new OracleScriptParser();
        }

        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            throw new NotImplementedException();

        }

        public override IEnumerable<QsiAnalyzerBase> CreateAnalyzers(QsiEngine engine)
        {
            throw new NotImplementedException();

        }
    }
}
