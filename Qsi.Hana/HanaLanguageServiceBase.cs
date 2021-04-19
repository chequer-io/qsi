using System;
using Qsi.Analyzers;
using Qsi.Parsing;
using Qsi.Parsing.Common;
using Qsi.Services;

namespace Qsi.Hana
{
    public abstract class HanaLanguageServiceBase : QsiLanguageServiceBase
    {
        public override IQsiTreeParser CreateTreeParser()
        {
            throw new NotImplementedException();
        }

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            throw new NotImplementedException();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new CommonScriptParser();
        }

        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new();
        }
    }
}
