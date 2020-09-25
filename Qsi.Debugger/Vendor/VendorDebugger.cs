using System;
using Qsi.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor
{
    internal abstract class VendorDebugger
    {
        public QsiEngine Engine => _engine.Value;

        public IRawTreeParser RawTreeParser => _rawTreeParser.Value;

        private readonly Lazy<QsiEngine> _engine;
        private readonly Lazy<IRawTreeParser> _rawTreeParser;

        protected VendorDebugger()
        {
            _engine = new Lazy<QsiEngine>(CreateEngine);
            _rawTreeParser = new Lazy<IRawTreeParser>(CreateRawTreeParser);
        }

        protected virtual QsiEngine CreateEngine()
        {
            return new QsiEngine(CreateLanguageService());
        }

        protected abstract IRawTreeParser CreateRawTreeParser();

        protected abstract IQsiLanguageService CreateLanguageService();
    }
}
