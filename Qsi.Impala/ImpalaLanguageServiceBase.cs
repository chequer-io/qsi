using System;
using Qsi.Analyzers;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.Impala
{
    public abstract class ImpalaLanguageServiceBase : QsiLanguageServiceBase
    {
        public abstract Version Version { get; }

        public override IQsiTreeParser CreateTreeParser()
        {
            return new ImpalaParser(Version);
        }

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            return new ImpalaDeparser();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new ImpalaScriptParser();
        }

        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new();
        }
    }
}
