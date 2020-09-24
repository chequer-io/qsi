using System;
using Qsi.Compiler;
using Qsi.Diagnostics;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.Debugger.Vendor
{
    internal abstract class VendorDebugger
    {
        public IQsiTreeParser Parser => _parser.Value;

        public IQsiScriptParser ScriptParser => _scriptParser.Value;

        public IRawTreeParser RawParser => _rawParser.Value;

        public IQsiLanguageService LanguageService => _languageService.Value;

        private readonly Lazy<IQsiTreeParser> _parser;
        private readonly Lazy<IQsiScriptParser> _scriptParser;
        private readonly Lazy<IRawTreeParser> _rawParser;
        private readonly Lazy<IQsiLanguageService> _languageService;

        protected VendorDebugger()
        {
            _languageService = new Lazy<IQsiLanguageService>(CreateLanguageService);
            _rawParser = new Lazy<IRawTreeParser>(CreateRawTreeParser);
            _parser = new Lazy<IQsiTreeParser>(() => _languageService.Value.CreateTreeParser());
            _scriptParser = new Lazy<IQsiScriptParser>(() => _languageService.Value.CreateScriptParser());
        }

        public abstract IQsiLanguageService CreateLanguageService();

        public abstract IRawTreeParser CreateRawTreeParser();

        public virtual QsiTableCompiler CreateCopmiler()
        {
            return new QsiTableCompiler(LanguageService);
        }
    }
}
