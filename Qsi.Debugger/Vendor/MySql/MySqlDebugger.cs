using System;
using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Data;
using Qsi.Diagnostics;
using Qsi.MySql;
using Qsi.MySql.Diagnostics;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.MySql
{
    internal class MySqlDebugger : VendorDebugger
    {
        private readonly Version _version;
        private readonly bool _useDelimiter;

        public MySqlDebugger(Version version, bool useDelimiter = true)
        {
            _version = version;
            _useDelimiter = useDelimiter;
        }

        protected override IQsiLanguageService CreateLanguageService()
        {
            var service = new MySqlLanguageService(_version);

            if (_useDelimiter)
                return service;

            return new MySqlLanguageServiceWrapper(service);
        }

        protected override IRawTreeParser CreateRawTreeParser()
        {
            return new MySqlRawParser(_version);
        }

        private class MySqlLanguageServiceWrapper : IQsiLanguageService
        {
            private readonly MySqlLanguageService _service;

            public MySqlLanguageServiceWrapper(MySqlLanguageService service)
            {
                _service = service;
            }

            public QsiAnalyzerOptions CreateAnalyzerOptions()
            {
                return _service.CreateAnalyzerOptions();
            }

            public IEnumerable<QsiAnalyzerBase> CreateAnalyzers(QsiEngine engine)
            {
                return _service.CreateAnalyzers(engine);
            }

            public IQsiTreeParser CreateTreeParser()
            {
                return _service.CreateTreeParser();
            }

            public IQsiTreeDeparser CreateTreeDeparser()
            {
                return _service.CreateTreeDeparser();
            }

            public IQsiScriptParser CreateScriptParser()
            {
                var scriptParser = (MySqlScriptParser)_service.CreateScriptParser();
                scriptParser.UseDelimiter = false;
                return scriptParser;
            }

            public IQsiRepositoryProvider CreateRepositoryProvider()
            {
                return _service.CreateRepositoryProvider();
            }

            public bool MatchIdentifier(QsiIdentifier x, QsiIdentifier y)
            {
                return _service.MatchIdentifier(x, y);
            }
        }
    }
}
